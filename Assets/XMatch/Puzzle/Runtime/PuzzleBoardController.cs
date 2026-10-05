using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public sealed class PuzzleBoardController : MonoBehaviour
    {
        private const float CellSpacing = 1f;
        private const float BoardVerticalOffset = -0.9f;
        private const float SwapDuration = 0.13f;
        private const float ClearDuration = 0.12f;
        private const float FallDuration = 0.18f;
        private const float SpawnDuration = 0.22f;
        private const float CascadePause = 0.05f;
        private const float SwipeThresholdPixels = 24f;

        private readonly Dictionary<BoardPosition, TileView>
            visuals =
                new Dictionary<BoardPosition, TileView>();

        private StageSession session;
        private Camera boardCamera;
        private Sprite tileSprite;
        private Texture2D tileTexture;
        private SpriteRenderer boardBackdrop;
        private int currentLevelIndex;
        private bool showLevelSelect = true;
        private bool inputLocked;
        private bool pointerArmed;
        private Vector2 pointerStartScreen;
        private BoardPosition pointerStartCell;
        private string transientBanner = string.Empty;
        private float bannerUntil;
        private BoosterKind selectedBooster = BoosterKind.None;
        private XMatchAudioDirector audioDirector;
        private Color screenFlashColor = Color.white;
        private float screenFlashAlpha;
        private int screenFlashVersion;
        private int cameraShakeVersion;

        private GUIStyle titleStyle;
        private GUIStyle infoStyle;
        private GUIStyle goalStyle;
        private GUIStyle statusStyle;
        private GUIStyle buttonStyle;

        private void Start()
        {
            Screen.orientation =
                ScreenOrientation.Portrait;

            try
            {
                EnsureCamera();
                CreateTileSprite();
                EnsureAudio();

                StartLevel(0);
                showLevelSelect = true;

                XMatchArtLibrary.Warmup();

                if (!string.IsNullOrEmpty(
                        XMatchArtLibrary.LoadError))
                {
                    ShowBanner(
                        "ART FALLBACK ACTIVE",
                        2.0f);
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);

                if (boardCamera == null)
                {
                    EnsureCamera();
                }

                transientBanner =
                    "STARTUP ERROR: " +
                    exception.GetType().Name +
                    " - " +
                    exception.Message;

                bannerUntil =
                    float.PositiveInfinity;

                showLevelSelect = false;
            }
        }

        private void Update()
        {
            if (session == null)
            {
                return;
            }

            if (!showLevelSelect &&
                !inputLocked &&
                session.Status == StageStatus.InProgress)
            {
                HandlePointerInput();
            }

            if (bannerUntil > 0f &&
                Time.unscaledTime > bannerUntil)
            {
                transientBanner = string.Empty;
                bannerUntil = 0f;
            }
        }

        private void OnDestroy()
        {
            if (tileSprite != null)
            {
                Destroy(tileSprite);
            }

            if (tileTexture != null)
            {
                Destroy(tileTexture);
            }
        }

        private void StartNewStage()
        {
            StartLevel(currentLevelIndex);
        }

        private void StartLevel(int levelIndex)
        {
            levelIndex = Mathf.Clamp(
                levelIndex,
                0,
                PrototypeLevelFactory.LevelCount - 1);

            StopAllCoroutines();
            inputLocked = false;
            pointerArmed = false;
            transientBanner = string.Empty;
            bannerUntil = 0f;
            selectedBooster = BoosterKind.None;
            currentLevelIndex = levelIndex;

            ClearVisuals();

            session =
                PrototypeLevelFactory.CreateStage(
                    currentLevelIndex);

            FitCamera();
            UpdateTheme();
            BuildVisuals();
            EnsureBoardBackdrop();

            ShowBanner(
                $"LEVEL {currentLevelIndex + 1}  " +
                PrototypeLevelFactory.GetTitle(currentLevelIndex),
                1.25f);
        }

        private void AdvanceLevel()
        {
            if (currentLevelIndex + 1 >=
                PrototypeLevelFactory.LevelCount)
            {
                showLevelSelect = true;
                return;
            }

            StartLevel(currentLevelIndex + 1);
        }

        private void UpdateTheme()
        {
            Color accent =
                ThemeColor(currentLevelIndex);

            boardCamera.backgroundColor =
                Color.Lerp(
                    new Color(0.025f, 0.018f, 0.050f),
                    accent,
                    0.10f);
        }

        private void EnsureBoardBackdrop()
        {
            if (boardBackdrop == null)
            {
                var backdropObject =
                    new GameObject("Board Backdrop");
                backdropObject.transform.SetParent(
                    transform,
                    worldPositionStays: false);
                backdropObject.transform.localPosition =
                    new Vector3(
                        0f,
                        BoardVerticalOffset,
                        0.65f);

                boardBackdrop =
                    backdropObject.AddComponent<SpriteRenderer>();
                boardBackdrop.sprite = tileSprite;
                boardBackdrop.sortingOrder = -10;
            }

            boardBackdrop.transform.localScale =
                new Vector3(
                    session.Board.Width + 0.55f,
                    session.Board.Height + 0.55f,
                    1f);

            Color accent =
                ThemeColor(currentLevelIndex);
            boardBackdrop.color =
                new Color(
                    accent.r * 0.22f,
                    accent.g * 0.22f,
                    accent.b * 0.28f,
                    0.92f);
        }

        private void EnsureCamera()
        {
            boardCamera = Camera.main;

            if (boardCamera == null)
            {
                var cameraObject =
                    new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                boardCamera =
                    cameraObject.AddComponent<Camera>();
            }

            boardCamera.orthographic = true;
            boardCamera.clearFlags =
                CameraClearFlags.SolidColor;
            boardCamera.backgroundColor =
                new Color(0.055f, 0.045f, 0.085f);
            boardCamera.transform.position =
                new Vector3(0f, BoardVerticalOffset, -10f);

            if (boardCamera.GetComponent<AudioListener>() == null)
            {
                boardCamera.gameObject.AddComponent<AudioListener>();
            }
        }

        private void EnsureAudio()
        {
            audioDirector =
                GetComponent<XMatchAudioDirector>();

            if (audioDirector == null)
            {
                audioDirector =
                    gameObject.AddComponent<XMatchAudioDirector>();
            }
        }

        private void FitCamera()
        {
            float aspect =
                Mathf.Max(0.35f, boardCamera.aspect);

            float halfBoardWidth =
                (session.Board.Width * CellSpacing * 0.5f) +
                0.35f;
            float halfBoardHeight =
                (session.Board.Height * CellSpacing * 0.5f) +
                0.35f;

            float sizeForWidth =
                halfBoardWidth / aspect;

            boardCamera.orthographicSize =
                Mathf.Max(
                    halfBoardHeight + 0.9f,
                    sizeForWidth);
        }

        private void CreateTileSprite()
        {
            const int size = 64;
            const float radius = 12f;

            tileTexture =
                new Texture2D(
                    size,
                    size,
                    TextureFormat.RGBA32,
                    false);

            tileTexture.name = "XMatch_RuntimeGem";
            tileTexture.filterMode = FilterMode.Bilinear;
            tileTexture.wrapMode = TextureWrapMode.Clamp;

            var pixels =
                new Color[size * size];

            float half = (size - 1) * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx =
                        Mathf.Max(
                            Mathf.Abs(x - half) -
                            (half - radius),
                            0f);
                    float dy =
                        Mathf.Max(
                            Mathf.Abs(y - half) -
                            (half - radius),
                            0f);
                    float distance =
                        Mathf.Sqrt((dx * dx) + (dy * dy));

                    float alpha =
                        Mathf.Clamp01(
                            (radius + 0.75f) - distance);

                    float vertical =
                        Mathf.Lerp(
                            0.78f,
                            1.08f,
                            y / (float)(size - 1));

                    float highlightDx =
                        (x - (size * 0.32f)) /
                        (size * 0.33f);
                    float highlightDy =
                        (y - (size * 0.72f)) /
                        (size * 0.24f);
                    float highlight =
                        Mathf.Clamp01(
                            1f -
                            Mathf.Sqrt(
                                (highlightDx * highlightDx) +
                                (highlightDy * highlightDy)));

                    float value =
                        Mathf.Clamp01(
                            vertical +
                            (highlight * 0.24f));

                    pixels[(y * size) + x] =
                        new Color(
                            value,
                            value,
                            value,
                            alpha);
                }
            }

            tileTexture.SetPixels(pixels);
            tileTexture.Apply();

            tileSprite =
                Sprite.Create(
                    tileTexture,
                    new Rect(0f, 0f, size, size),
                    new Vector2(0.5f, 0.5f),
                    size);

            tileSprite.name = "XMatch_RuntimeGemSprite";
        }

        private void BuildVisuals()
        {
            for (int y = 0; y < session.Board.Height; y++)
            {
                for (int x = 0; x < session.Board.Width; x++)
                {
                    var position =
                        new BoardPosition(x, y);

                    TileKind kind =
                        session.Board.Get(position);

                    if (kind == TileKind.Empty)
                    {
                        continue;
                    }

                    TileView view =
                        CreateTileView(
                            kind,
                            session.Board.GetPowerUp(position),
                            BoardToWorld(position));

                    visuals[position] = view;
                }
            }
        }

        private TileView CreateTileView(
            TileKind kind,
            PowerUpKind powerUp,
            Vector3 worldPosition)
        {
            var tileObject =
                new GameObject(
                    $"Tile_{kind}");

            tileObject.transform.SetParent(
                transform,
                worldPositionStays: true);
            tileObject.transform.position =
                worldPosition;

            TileView view =
                tileObject.AddComponent<TileView>();
            view.Initialize(
                kind,
                powerUp,
                tileSprite);

            return view;
        }

        private void ClearVisuals()
        {
            foreach (
                KeyValuePair<BoardPosition, TileView> pair
                in visuals)
            {
                if (pair.Value != null)
                {
                    Destroy(pair.Value.gameObject);
                }
            }

            visuals.Clear();
        }

        private void HandlePointerInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (IsScreenPointOverBoosterBar(touch.position))
                {
                    pointerArmed = false;
                    return;
                }

                if (selectedBooster != BoosterKind.None)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        UseSelectedBooster(touch.position);
                    }

                    return;
                }

                if (touch.phase == TouchPhase.Began)
                {
                    ArmPointer(touch.position);
                }
                else if (
                    touch.phase == TouchPhase.Ended ||
                    touch.phase == TouchPhase.Canceled)
                {
                    ReleasePointer(touch.position);
                }

                return;
            }

            if (selectedBooster != BoosterKind.None)
            {
                if (Input.GetMouseButtonUp(0) &&
                    !IsScreenPointOverBoosterBar(Input.mousePosition))
                {
                    UseSelectedBooster(Input.mousePosition);
                }

                return;
            }

            if (Input.GetMouseButtonDown(0) &&
                !IsScreenPointOverBoosterBar(Input.mousePosition))
            {
                ArmPointer(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0) &&
                !IsScreenPointOverBoosterBar(Input.mousePosition))
            {
                ReleasePointer(Input.mousePosition);
            }
        }

        private void UseSelectedBooster(Vector2 screenPosition)
        {
            BoardPosition target;

            if (!TryScreenToBoard(screenPosition, out target))
            {
                return;
            }

            BoosterKind booster = selectedBooster;
            selectedBooster = BoosterKind.None;

            if (audioDirector != null)
            {
                audioDirector.Play(
                    XMatchSoundKind.Booster,
                    0.72f);
            }

            PlayBoosterEffect(
                booster,
                target);

            CascadeResult cascades =
                session.UseBooster(booster, target);

            StartCoroutine(
                ResolveBoosterRoutine(
                    booster,
                    cascades));
        }

        private IEnumerator ResolveBoosterRoutine(
            BoosterKind booster,
            CascadeResult cascades)
        {
            inputLocked = true;
            ShowBanner(
                BoosterLabel(booster),
                0.7f);

            for (int i = 0;
                 i < cascades.Steps.Count;
                 i++)
            {
                CascadeStep step = cascades.Steps[i];

                PlayCascadeFeedback(step);
                ApplyCreatedPowerUpVisual(step);
                yield return AnimateClear(step);
                yield return AnimateGravity(step);
                yield return AnimateSpawns(step);

                if (CascadePause > 0f)
                {
                    yield return new WaitForSeconds(
                        CascadePause);
                }
            }

            if (session.Status == StageStatus.InProgress &&
                session.NeedsShuffle)
            {
                yield return AnimateShuffle();
            }

            PlayResultFeedback(session.Status);

            EnsurePresentationMatchesBoard();
            inputLocked = false;
        }

        private void ArmPointer(Vector2 screenPosition)
        {
            BoardPosition position;

            if (!TryScreenToBoard(
                    screenPosition,
                    out position))
            {
                pointerArmed = false;
                return;
            }

            pointerArmed = true;
            pointerStartScreen = screenPosition;
            pointerStartCell = position;

            if (audioDirector != null)
            {
                audioDirector.Play(
                    XMatchSoundKind.Tap,
                    0.30f);
            }
        }

        private void ReleasePointer(Vector2 screenPosition)
        {
            if (!pointerArmed)
            {
                return;
            }

            pointerArmed = false;

            Vector2 delta =
                screenPosition - pointerStartScreen;

            if (delta.magnitude <
                SwipeThresholdPixels)
            {
                TryActivateTappedPowerUp(
                    pointerStartCell);
                return;
            }

            int dx = 0;
            int dy = 0;

            if (Mathf.Abs(delta.x) >=
                Mathf.Abs(delta.y))
            {
                dx = delta.x >= 0f ? 1 : -1;
            }
            else
            {
                dy = delta.y >= 0f ? 1 : -1;
            }

            var target =
                new BoardPosition(
                    pointerStartCell.X + dx,
                    pointerStartCell.Y + dy);

            if (!session.Board.IsInside(target))
            {
                return;
            }

            StartCoroutine(
                ResolveMoveRoutine(
                    pointerStartCell,
                    target));
        }

        private void TryActivateTappedPowerUp(
            BoardPosition position)
        {
            if (!session.Board.IsInside(position) ||
                session.Board.GetPowerUp(position) ==
                PowerUpKind.None)
            {
                return;
            }

            StartCoroutine(
                ResolveTappedPowerUpRoutine(
                    position));
        }

        private IEnumerator ResolveTappedPowerUpRoutine(
            BoardPosition position)
        {
            inputLocked = true;

            PowerUpKind powerUp =
                session.Board.GetPowerUp(position);

            StageTurnResult turn =
                session.TryActivatePowerUp(
                    position);

            if (!turn.Accepted)
            {
                inputLocked = false;
                yield break;
            }

            ShowBanner(
                PowerUpLabel(powerUp) +
                "  TAP!",
                0.75f);

            for (int i = 0;
                 i < turn.Move.Cascades.Steps.Count;
                 i++)
            {
                CascadeStep step =
                    turn.Move.Cascades.Steps[i];

                PlayCascadeFeedback(step);
                ApplyCreatedPowerUpVisual(step);

                yield return AnimateClear(step);
                yield return AnimateGravity(step);
                yield return AnimateSpawns(step);

                if (CascadePause > 0f)
                {
                    yield return
                        new WaitForSeconds(
                            CascadePause);
                }
            }

            if (turn.Status ==
                StageStatus.InProgress &&
                turn.NeedsShuffle)
            {
                yield return AnimateShuffle();
            }

            PlayResultFeedback(turn.Status);

            EnsurePresentationMatchesBoard();
            inputLocked = false;
        }

        private IEnumerator ResolveMoveRoutine(
            BoardPosition from,
            BoardPosition to)
        {
            inputLocked = true;

            TileView fromView;
            TileView toView;

            if (!visuals.TryGetValue(
                    from,
                    out fromView) ||
                !visuals.TryGetValue(
                    to,
                    out toView))
            {
                RebuildFromLogicalBoard();
                inputLocked = false;
                yield break;
            }

            StageTurnResult turn =
                session.TryMove(from, to);

            if (!turn.Accepted)
            {
                yield return AnimateInvalidSwap(
                    fromView,
                    toView,
                    from,
                    to);

                if (audioDirector != null)
                {
                    audioDirector.Play(
                        XMatchSoundKind.Invalid,
                        0.62f);
                }

                ShowBanner("NO MATCH", 0.7f);
                inputLocked = false;
                yield break;
            }

            if (audioDirector != null)
            {
                audioDirector.Play(
                    XMatchSoundKind.Swap,
                    0.42f);
            }

            SwapVisualMapping(
                from,
                to,
                fromView,
                toView);

            yield return AnimateTwoToBoardCells(
                fromView,
                to,
                toView,
                from,
                SwapDuration);

            for (int i = 0;
                 i < turn.Move.Cascades.Steps.Count;
                 i++)
            {
                CascadeStep step =
                    turn.Move.Cascades.Steps[i];

                if (step.ChainNumber > 1)
                {
                    ShowBanner(
                        $"COMBO x{step.ChainNumber}",
                        0.6f);
                }

                PlayCascadeFeedback(step);
                ApplyCreatedPowerUpVisual(step);
                yield return AnimateClear(step);
                yield return AnimateGravity(step);
                yield return AnimateSpawns(step);

                if (CascadePause > 0f)
                {
                    yield return new WaitForSeconds(
                        CascadePause);
                }
            }

            if (turn.Status ==
                StageStatus.InProgress &&
                turn.NeedsShuffle)
            {
                yield return AnimateShuffle();
            }

            if (turn.Status == StageStatus.Won)
            {
                ShowBanner("CLEAR!", 5f);
            }
            else if (turn.Status == StageStatus.Lost)
            {
                ShowBanner("OUT OF MOVES", 5f);
            }

            PlayResultFeedback(turn.Status);

            EnsurePresentationMatchesBoard();
            inputLocked = false;
        }

        private IEnumerator AnimateInvalidSwap(
            TileView a,
            TileView b,
            BoardPosition aCell,
            BoardPosition bCell)
        {
            Vector3 aStart =
                BoardToWorld(aCell);
            Vector3 bStart =
                BoardToWorld(bCell);

            yield return AnimateTwoPositions(
                a,
                bStart,
                b,
                aStart,
                SwapDuration);

            yield return AnimateTwoPositions(
                a,
                aStart,
                b,
                bStart,
                SwapDuration);
        }

        private IEnumerator AnimateTwoToBoardCells(
            TileView first,
            BoardPosition firstTarget,
            TileView second,
            BoardPosition secondTarget,
            float duration)
        {
            yield return AnimateTwoPositions(
                first,
                BoardToWorld(firstTarget),
                second,
                BoardToWorld(secondTarget),
                duration);
        }

        private IEnumerator AnimateTwoPositions(
            TileView first,
            Vector3 firstTarget,
            TileView second,
            Vector3 secondTarget,
            float duration)
        {
            Vector3 firstStart =
                first.transform.position;
            Vector3 secondStart =
                second.transform.position;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t =
                    Mathf.Clamp01(elapsed / duration);
                float eased =
                    Smooth01(t);

                first.transform.position =
                    Vector3.Lerp(
                        firstStart,
                        firstTarget,
                        eased);
                second.transform.position =
                    Vector3.Lerp(
                        secondStart,
                        secondTarget,
                        eased);

                yield return null;
            }

            first.transform.position =
                firstTarget;
            second.transform.position =
                secondTarget;
        }

        private IEnumerator AnimateClear(
            CascadeStep step)
        {
            var clearing =
                new List<TileView>();

            for (int i = 0;
                 i < step.Cleared.Count;
                 i++)
            {
                ClearedTile tile =
                    step.Cleared[i];

                PlayClearEffect(tile);

                TileView view;

                if (visuals.TryGetValue(
                        tile.Position,
                        out view))
                {
                    clearing.Add(view);
                }
            }

            float elapsed = 0f;

            while (elapsed < ClearDuration)
            {
                elapsed += Time.deltaTime;
                float t =
                    Mathf.Clamp01(
                        elapsed / ClearDuration);

                float pulse =
                    1f + (Mathf.Sin(t * Mathf.PI) * 0.12f);
                float shrink =
                    Mathf.Lerp(pulse, 0.02f, t);

                for (int i = 0;
                     i < clearing.Count;
                     i++)
                {
                    if (clearing[i] != null)
                    {
                        clearing[i].SetScaleFactor(
                            shrink);
                    }
                }

                yield return null;
            }

            for (int i = 0;
                 i < step.Cleared.Count;
                 i++)
            {
                BoardPosition position =
                    step.Cleared[i].Position;

                TileView view;

                if (visuals.TryGetValue(
                        position,
                        out view))
                {
                    visuals.Remove(position);

                    if (view != null)
                    {
                        Destroy(view.gameObject);
                    }
                }
            }
        }

        private IEnumerator AnimateGravity(
            CascadeStep step)
        {
            if (step.Moved.Count == 0)
            {
                yield break;
            }

            var animations =
                new List<MoveAnimation>(
                    step.Moved.Count);

            for (int i = 0;
                 i < step.Moved.Count;
                 i++)
            {
                TileMove move =
                    step.Moved[i];

                TileView view;

                if (!visuals.TryGetValue(
                        move.From,
                        out view))
                {
                    continue;
                }

                visuals.Remove(move.From);

                animations.Add(
                    new MoveAnimation(
                        view,
                        view.transform.position,
                        BoardToWorld(move.To),
                        move.To));
            }

            for (int i = 0;
                 i < animations.Count;
                 i++)
            {
                MoveAnimation animation =
                    animations[i];

                visuals[animation.TargetCell] =
                    animation.View;
            }

            yield return AnimateMany(
                animations,
                FallDuration);
        }

        private IEnumerator AnimateSpawns(
            CascadeStep step)
        {
            if (step.Spawned.Count == 0)
            {
                yield break;
            }

            var animations =
                new List<MoveAnimation>(
                    step.Spawned.Count);

            float topWorldY =
                BoardToWorld(
                    new BoardPosition(
                        0,
                        session.Board.Height - 1)).y;

            for (int i = 0;
                 i < step.Spawned.Count;
                 i++)
            {
                TileSpawn spawn =
                    step.Spawned[i];

                Vector3 target =
                    BoardToWorld(spawn.Position);

                float extraHeight =
                    1.2f +
                    ((session.Board.Height -
                      spawn.Position.Y) * 0.18f);

                Vector3 start =
                    new Vector3(
                        target.x,
                        topWorldY + extraHeight,
                        target.z);

                TileView view =
                    CreateTileView(
                        spawn.Kind,
                        PowerUpKind.None,
                        start);

                visuals[spawn.Position] = view;

                animations.Add(
                    new MoveAnimation(
                        view,
                        start,
                        target,
                        spawn.Position));
            }

            yield return AnimateMany(
                animations,
                SpawnDuration);
        }

        private IEnumerator AnimateMany(
            List<MoveAnimation> animations,
            float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t =
                    Mathf.Clamp01(elapsed / duration);
                float eased =
                    Smooth01(t);

                for (int i = 0;
                     i < animations.Count;
                     i++)
                {
                    MoveAnimation animation =
                        animations[i];

                    if (animation.View == null)
                    {
                        continue;
                    }

                    animation.View.transform.position =
                        Vector3.Lerp(
                            animation.Start,
                            animation.Target,
                            eased);
                }

                yield return null;
            }

            for (int i = 0;
                 i < animations.Count;
                 i++)
            {
                if (animations[i].View != null)
                {
                    animations[i].View.transform.position =
                        animations[i].Target;
                }
            }
        }

        private IEnumerator AnimateShuffle()
        {
            ShowBanner("SHUFFLE", 1.1f);

            var current =
                new List<TileView>(
                    visuals.Values);

            float duration = 0.18f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t =
                    Mathf.Clamp01(elapsed / duration);

                for (int i = 0;
                     i < current.Count;
                     i++)
                {
                    if (current[i] != null)
                    {
                        current[i].SetScaleFactor(
                            Mathf.Lerp(
                                1f,
                                0.08f,
                                t));
                    }
                }

                yield return null;
            }

            bool shuffled =
                session.TryShuffleBoard();

            if (!shuffled)
            {
                RebuildFromLogicalBoard();
                ShowBanner(
                    "SHUFFLE FAILED",
                    1.2f);
                yield break;
            }

            ClearVisuals();
            BuildVisuals();

            current =
                new List<TileView>(
                    visuals.Values);

            for (int i = 0;
                 i < current.Count;
                 i++)
            {
                current[i].SetScaleFactor(0.08f);
            }

            elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t =
                    Mathf.Clamp01(elapsed / duration);
                float eased =
                    Smooth01(t);

                for (int i = 0;
                     i < current.Count;
                     i++)
                {
                    if (current[i] != null)
                    {
                        current[i].SetScaleFactor(
                            Mathf.Lerp(
                                0.08f,
                                1f,
                                eased));
                    }
                }

                yield return null;
            }

            for (int i = 0;
                 i < current.Count;
                 i++)
            {
                if (current[i] != null)
                {
                    current[i].SetScaleFactor(1f);
                }
            }
        }

        private void SwapVisualMapping(
            BoardPosition from,
            BoardPosition to,
            TileView fromView,
            TileView toView)
        {
            visuals[from] = toView;
            visuals[to] = fromView;
        }

        private bool TryScreenToBoard(
            Vector2 screenPosition,
            out BoardPosition position)
        {
            Vector3 world =
                boardCamera.ScreenToWorldPoint(
                    new Vector3(
                        screenPosition.x,
                        screenPosition.y,
                        -boardCamera.transform.position.z));

            float originX =
                -((session.Board.Width - 1) *
                  CellSpacing *
                  0.5f);
            float originY =
                -((session.Board.Height - 1) *
                  CellSpacing *
                  0.5f) +
                BoardVerticalOffset;

            int x =
                Mathf.RoundToInt(
                    (world.x - originX) /
                    CellSpacing);
            int y =
                Mathf.RoundToInt(
                    (world.y - originY) /
                    CellSpacing);

            position =
                new BoardPosition(x, y);

            if (!session.Board.IsInside(position))
            {
                return false;
            }

            Vector3 center =
                BoardToWorld(position);

            return Mathf.Abs(world.x - center.x) <=
                       CellSpacing * 0.5f &&
                   Mathf.Abs(world.y - center.y) <=
                       CellSpacing * 0.5f;
        }

        private Vector3 BoardToWorld(
            BoardPosition position)
        {
            float originX =
                -((session.Board.Width - 1) *
                  CellSpacing *
                  0.5f);
            float originY =
                -((session.Board.Height - 1) *
                  CellSpacing *
                  0.5f) +
                BoardVerticalOffset;

            return new Vector3(
                originX +
                (position.X * CellSpacing),
                originY +
                (position.Y * CellSpacing),
                0f);
        }

        private void RebuildFromLogicalBoard()
        {
            ClearVisuals();
            BuildVisuals();
        }

        private void EnsurePresentationMatchesBoard()
        {
            bool mismatch =
                visuals.Count !=
                session.Board.Width *
                session.Board.Height;

            if (!mismatch)
            {
                for (int y = 0;
                     y < session.Board.Height &&
                     !mismatch;
                     y++)
                {
                    for (int x = 0;
                         x < session.Board.Width;
                         x++)
                    {
                        var position =
                            new BoardPosition(x, y);

                        TileView view;

                        if (!visuals.TryGetValue(
                                position,
                                out view) ||
                            view == null ||
                            view.Kind !=
                            session.Board.Get(position) ||
                            view.PowerUp !=
                            session.Board.GetPowerUp(position))
                        {
                            mismatch = true;
                            break;
                        }
                    }
                }
            }

            if (mismatch)
            {
                RebuildFromLogicalBoard();
                ShowBanner(
                    "BOARD RESYNC",
                    0.8f);
            }
        }

        private void ApplyCreatedPowerUpVisual(
            CascadeStep step)
        {
            if (!step.CreatedPowerUp.HasValue)
            {
                return;
            }

            PowerUpCreation creation =
                step.CreatedPowerUp.Value;

            TileView view;

            if (visuals.TryGetValue(
                    creation.Position,
                    out view) &&
                view != null)
            {
                view.SetPowerUp(
                    creation.Kind,
                    creation.PowerUp);

                if (audioDirector != null)
                {
                    audioDirector.Play(
                        XMatchSoundKind.CreateSpecial,
                        0.82f);
                }

                FlashScreen(
                    new Color(
                        1f,
                        0.55f,
                        0.92f,
                        1f),
                    0.20f,
                    0.15f);

                SpawnSparkBurst(
                    creation.Position,
                    10,
                    new Color(
                        1f,
                        0.68f,
                        0.95f,
                        1f),
                    2.8f,
                    0.42f);

                PlayVfx(
                    XMatchVfxKind.MagicCircle,
                    creation.Position,
                    0.36f,
                    new Vector3(1.55f, 1.55f, 1f));

                ShowBanner(
                    PowerUpLabel(creation.PowerUp),
                    0.8f);
            }
        }

        private void PlayClearEffect(
            ClearedTile tile)
        {
            XMatchVfxKind effect =
                XMatchVfxKind.PopSmall;

            float duration = 0.30f;
            Vector3 scale =
                new Vector3(
                    1.30f,
                    1.30f,
                    1f);

            Color sparkColor =
                TileSparkColor(tile.Kind);

            int sparkCount = 2;
            float sparkSpeed = 1.7f;
            float sparkDuration = 0.30f;

            switch (tile.PowerUp)
            {
                case PowerUpKind.RowBlast:
                    effect =
                        XMatchVfxKind.RowBlast;
                    duration = 0.38f;
                    scale =
                        new Vector3(
                            session.Board.Width * 1.10f,
                            1.35f,
                            1f);

                    sparkCount = 12;
                    sparkSpeed = 4.0f;
                    sparkDuration = 0.42f;
                    sparkColor =
                        new Color(
                            0.32f,
                            0.95f,
                            1f,
                            1f);

                    PlaySpecialSound(
                        PowerUpKind.RowBlast);

                    FlashScreen(
                        sparkColor,
                        0.18f,
                        0.16f);

                    ShakeCamera(
                        0.12f,
                        0.09f);
                    break;

                case PowerUpKind.ColumnBlast:
                    effect =
                        XMatchVfxKind.ColumnBlast;
                    duration = 0.38f;
                    scale =
                        new Vector3(
                            1.35f,
                            session.Board.Height * 1.10f,
                            1f);

                    sparkCount = 12;
                    sparkSpeed = 4.0f;
                    sparkDuration = 0.42f;
                    sparkColor =
                        new Color(
                            1f,
                            0.78f,
                            0.24f,
                            1f);

                    PlaySpecialSound(
                        PowerUpKind.ColumnBlast);

                    FlashScreen(
                        sparkColor,
                        0.18f,
                        0.16f);

                    ShakeCamera(
                        0.12f,
                        0.09f);
                    break;

                case PowerUpKind.Bomb:
                    effect =
                        XMatchVfxKind.BombBurst;
                    duration = 0.48f;
                    scale =
                        new Vector3(
                            3.25f,
                            3.25f,
                            1f);

                    sparkCount = 18;
                    sparkSpeed = 4.8f;
                    sparkDuration = 0.52f;
                    sparkColor =
                        new Color(
                            1f,
                            0.34f,
                            0.20f,
                            1f);

                    PlaySpecialSound(
                        PowerUpKind.Bomb);

                    PlayVfx(
                        XMatchVfxKind.PopBig,
                        tile.Position,
                        0.40f,
                        new Vector3(
                            2.20f,
                            2.20f,
                            1f));

                    PlayVfx(
                        XMatchVfxKind.MagicCircle,
                        tile.Position,
                        0.46f,
                        new Vector3(
                            2.65f,
                            2.65f,
                            1f));

                    FlashScreen(
                        new Color(
                            1f,
                            0.54f,
                            0.22f,
                            1f),
                        0.26f,
                        0.28f);

                    ShakeCamera(
                        0.22f,
                        0.18f);
                    break;

                case PowerUpKind.ColorOrb:
                    effect =
                        XMatchVfxKind.ColorOrbBurst;
                    duration = 0.64f;
                    scale =
                        new Vector3(
                            4.15f,
                            4.15f,
                            1f);

                    sparkCount = 24;
                    sparkSpeed = 5.2f;
                    sparkDuration = 0.62f;
                    sparkColor =
                        new Color(
                            0.92f,
                            0.52f,
                            1f,
                            1f);

                    PlaySpecialSound(
                        PowerUpKind.ColorOrb);

                    PlayVfx(
                        XMatchVfxKind.MagicCircle,
                        tile.Position,
                        0.68f,
                        new Vector3(
                            4.7f,
                            4.7f,
                            1f));

                    PlayVfx(
                        XMatchVfxKind.PopBig,
                        tile.Position,
                        0.54f,
                        new Vector3(
                            3.10f,
                            3.10f,
                            1f));

                    FlashScreen(
                        Color.white,
                        0.34f,
                        0.38f);

                    ShakeCamera(
                        0.30f,
                        0.24f);
                    break;

                case PowerUpKind.Seeker:
                    effect =
                        XMatchVfxKind.SeekerDash;
                    duration = 0.40f;
                    scale =
                        new Vector3(
                            2.15f,
                            2.15f,
                            1f);

                    sparkCount = 10;
                    sparkSpeed = 3.6f;
                    sparkDuration = 0.42f;
                    sparkColor =
                        new Color(
                            0.50f,
                            1f,
                            0.62f,
                            1f);

                    PlaySpecialSound(
                        PowerUpKind.Seeker);

                    FlashScreen(
                        sparkColor,
                        0.14f,
                        0.12f);

                    ShakeCamera(
                        0.10f,
                        0.07f);
                    break;

                default:
                    if (tile.Kind == TileKind.Heart)
                    {
                        effect =
                            XMatchVfxKind.HeartBurst;
                        scale =
                            new Vector3(
                                1.52f,
                                1.52f,
                                1f);
                    }
                    else if (tile.Kind == TileKind.Rose)
                    {
                        effect =
                            XMatchVfxKind.RoseBurst;
                        scale =
                            new Vector3(
                                1.52f,
                                1.52f,
                                1f);
                    }
                    break;
            }

            PlayVfx(
                effect,
                tile.Position,
                duration,
                scale);

            SpawnSparkBurst(
                tile.Position,
                sparkCount,
                sparkColor,
                sparkSpeed,
                sparkDuration);
        }

        private void PlayCascadeFeedback(
            CascadeStep step)
        {
            if (audioDirector != null)
            {
                audioDirector.PlayMatch(
                    step.ChainNumber);
            }

            if (step.ChainNumber <= 1)
            {
                return;
            }

            float strength =
                Mathf.Clamp(
                    0.05f +
                    ((step.ChainNumber - 1) * 0.025f),
                    0.05f,
                    0.16f);

            FlashScreen(
                ThemeColor(currentLevelIndex),
                0.12f,
                strength);

            if (step.ChainNumber >= 3)
            {
                ShakeCamera(
                    0.10f,
                    Mathf.Min(
                        0.06f +
                        (step.ChainNumber * 0.012f),
                        0.14f));
            }
        }

        private void PlaySpecialSound(
            PowerUpKind powerUp)
        {
            if (audioDirector == null)
            {
                return;
            }

            switch (powerUp)
            {
                case PowerUpKind.RowBlast:
                    audioDirector.Play(
                        XMatchSoundKind.RowBlast,
                        0.90f);
                    break;

                case PowerUpKind.ColumnBlast:
                    audioDirector.Play(
                        XMatchSoundKind.ColumnBlast,
                        0.90f);
                    break;

                case PowerUpKind.Bomb:
                    audioDirector.Play(
                        XMatchSoundKind.Bomb,
                        1f);
                    break;

                case PowerUpKind.ColorOrb:
                    audioDirector.Play(
                        XMatchSoundKind.ColorOrb,
                        1f);
                    break;

                case PowerUpKind.Seeker:
                    audioDirector.Play(
                        XMatchSoundKind.Seeker,
                        0.88f);
                    break;
            }
        }

        private void PlayResultFeedback(
            StageStatus status)
        {
            if (status == StageStatus.Won)
            {
                if (audioDirector != null)
                {
                    audioDirector.Play(
                        XMatchSoundKind.Clear,
                        1f);
                }

                BoardPosition center =
                    new BoardPosition(
                        session.Board.Width / 2,
                        session.Board.Height / 2);

                PlayVfx(
                    XMatchVfxKind.PopBig,
                    center,
                    0.82f,
                    new Vector3(
                        6.2f,
                        6.2f,
                        1f));

                PlayVfx(
                    XMatchVfxKind.MagicCircle,
                    center,
                    0.92f,
                    new Vector3(
                        5.0f,
                        5.0f,
                        1f));

                SpawnSparkBurst(
                    center,
                    28,
                    new Color(
                        1f,
                        0.80f,
                        0.28f,
                        1f),
                    5.4f,
                    0.90f);

                FlashScreen(
                    new Color(
                        1f,
                        0.84f,
                        0.36f,
                        1f),
                    0.44f,
                    0.40f);

                ShakeCamera(
                    0.26f,
                    0.18f);
            }
            else if (status == StageStatus.Lost)
            {
                if (audioDirector != null)
                {
                    audioDirector.Play(
                        XMatchSoundKind.Fail,
                        0.80f);
                }

                FlashScreen(
                    new Color(
                        0.60f,
                        0.08f,
                        0.16f,
                        1f),
                    0.28f,
                    0.22f);
            }
        }

        private static Color TileSparkColor(
            TileKind kind)
        {
            switch (kind)
            {
                case TileKind.Heart:
                    return new Color(
                        1f,
                        0.24f,
                        0.34f,
                        1f);

                case TileKind.Lips:
                    return new Color(
                        1f,
                        0.25f,
                        0.78f,
                        1f);

                case TileKind.Diamond:
                    return new Color(
                        0.24f,
                        0.86f,
                        1f,
                        1f);

                case TileKind.Perfume:
                    return new Color(
                        0.72f,
                        0.42f,
                        1f,
                        1f);

                case TileKind.Rose:
                    return new Color(
                        0.42f,
                        1f,
                        0.42f,
                        1f);

                default:
                    return Color.white;
            }
        }

        private void PlayBoosterEffect(
            BoosterKind booster,
            BoardPosition target)
        {
            XMatchVfxKind effect =
                XMatchVfxKind.PopBig;

            Vector3 scale =
                new Vector3(2.0f, 2.0f, 1f);

            switch (booster)
            {
                case BoosterKind.RowClear:
                    effect = XMatchVfxKind.RowBlast;
                    scale =
                        new Vector3(4.8f, 1.25f, 1f);
                    break;

                case BoosterKind.ColumnClear:
                    effect =
                        XMatchVfxKind.ColumnBlast;
                    scale =
                        new Vector3(1.25f, 4.8f, 1f);
                    break;

                case BoosterKind.Hammer:
                    effect = XMatchVfxKind.PopBig;
                    break;
            }

            PlayVfx(
                effect,
                target,
                0.36f,
                scale);
        }

        private void PlayVfx(
            XMatchVfxKind effect,
            BoardPosition position,
            float duration,
            Vector3 targetScale)
        {
            Sprite sprite =
                XMatchArtLibrary.GetVfxSprite(effect);

            if (sprite == null)
            {
                return;
            }

            var effectObject =
                new GameObject(
                    "FX_" + effect);

            effectObject.transform.SetParent(
                transform,
                worldPositionStays: true);

            effectObject.transform.position =
                BoardToWorld(position) +
                new Vector3(0f, 0f, -0.25f);

            var renderer =
                effectObject.AddComponent<SpriteRenderer>();

            renderer.sprite = sprite;
            renderer.sortingOrder = 30;
            renderer.color =
                new Color(1f, 1f, 1f, 0f);

            StartCoroutine(
                AnimateVfx(
                    effectObject,
                    renderer,
                    duration,
                    targetScale));
        }

        private IEnumerator AnimateVfx(
            GameObject effectObject,
            SpriteRenderer renderer,
            float duration,
            Vector3 targetScale)
        {
            float elapsed = 0f;

            Vector3 startScale =
                targetScale * 0.52f;

            effectObject.transform.localScale =
                startScale;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t =
                    Mathf.Clamp01(elapsed / duration);

                float pop =
                    1f -
                    Mathf.Pow(1f - t, 3f);

                float alpha;

                if (t < 0.18f)
                {
                    alpha =
                        Mathf.Clamp01(t / 0.18f);
                }
                else
                {
                    alpha =
                        Mathf.Clamp01(
                            1f -
                            ((t - 0.18f) / 0.82f));
                }

                renderer.color =
                    new Color(
                        1f,
                        1f,
                        1f,
                        alpha);

                effectObject.transform.localScale =
                    Vector3.Lerp(
                        startScale,
                        targetScale,
                        pop);

                effectObject.transform.Rotate(
                    0f,
                    0f,
                    42f *
                    Time.unscaledDeltaTime);

                yield return null;
            }

            Destroy(effectObject);
        }

        private void SpawnSparkBurst(
            BoardPosition position,
            int count,
            Color color,
            float speed,
            float duration)
        {
            if (count <= 0 ||
                tileSprite == null)
            {
                return;
            }

            StartCoroutine(
                AnimateSparkBurst(
                    BoardToWorld(position),
                    count,
                    color,
                    speed,
                    duration));
        }

        private IEnumerator AnimateSparkBurst(
            Vector3 origin,
            int count,
            Color color,
            float speed,
            float duration)
        {
            var objects =
                new GameObject[count];

            var renderers =
                new SpriteRenderer[count];

            var velocities =
                new Vector3[count];

            var spin =
                new float[count];

            var baseScale =
                new float[count];

            for (int i = 0; i < count; i++)
            {
                var sparkle =
                    new GameObject("FX_Spark");

                sparkle.transform.SetParent(
                    transform,
                    worldPositionStays: true);

                sparkle.transform.position =
                    origin +
                    new Vector3(
                        0f,
                        0f,
                        -0.30f);

                var renderer =
                    sparkle.AddComponent<SpriteRenderer>();

                renderer.sprite = tileSprite;
                renderer.sortingOrder = 38;

                Color startColor = color;
                startColor.a = 0.95f;
                renderer.color = startColor;

                Vector2 direction =
                    UnityEngine.Random.insideUnitCircle;

                if (direction.sqrMagnitude <
                    0.001f)
                {
                    direction = Vector2.up;
                }

                direction.Normalize();

                float particleSpeed =
                    speed *
                    UnityEngine.Random.Range(
                        0.58f,
                        1.18f);

                velocities[i] =
                    new Vector3(
                        direction.x,
                        direction.y,
                        0f) *
                    particleSpeed;

                spin[i] =
                    UnityEngine.Random.Range(
                        -420f,
                        420f);

                baseScale[i] =
                    UnityEngine.Random.Range(
                        0.055f,
                        0.14f);

                sparkle.transform.localScale =
                    Vector3.one *
                    baseScale[i];

                objects[i] = sparkle;
                renderers[i] = renderer;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                float delta =
                    Time.unscaledDeltaTime;

                elapsed += delta;

                float t =
                    Mathf.Clamp01(
                        elapsed / duration);

                float fade =
                    1f -
                    (t * t);

                for (int i = 0; i < count; i++)
                {
                    if (objects[i] == null)
                    {
                        continue;
                    }

                    objects[i].transform.position +=
                        velocities[i] * delta;

                    velocities[i] *=
                        Mathf.Pow(
                            0.06f,
                            delta);

                    objects[i].transform.Rotate(
                        0f,
                        0f,
                        spin[i] * delta);

                    float scale =
                        baseScale[i] *
                        Mathf.Lerp(
                            1.25f,
                            0.18f,
                            t);

                    objects[i].transform.localScale =
                        Vector3.one * scale;

                    Color current = color;
                    current.a = fade;
                    renderers[i].color = current;
                }

                yield return null;
            }

            for (int i = 0; i < count; i++)
            {
                if (objects[i] != null)
                {
                    Destroy(objects[i]);
                }
            }
        }

        private void FlashScreen(
            Color color,
            float duration,
            float peakAlpha)
        {
            screenFlashVersion++;

            StartCoroutine(
                AnimateScreenFlash(
                    color,
                    duration,
                    peakAlpha,
                    screenFlashVersion));
        }

        private IEnumerator AnimateScreenFlash(
            Color color,
            float duration,
            float peakAlpha,
            int version)
        {
            screenFlashColor = color;
            screenFlashAlpha =
                Mathf.Clamp01(peakAlpha);

            float elapsed = 0f;

            while (elapsed < duration &&
                   version == screenFlashVersion)
            {
                elapsed +=
                    Time.unscaledDeltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed / duration);

                screenFlashAlpha =
                    Mathf.Clamp01(
                        peakAlpha *
                        (1f - (t * t)));

                yield return null;
            }

            if (version == screenFlashVersion)
            {
                screenFlashAlpha = 0f;
            }
        }

        private void ShakeCamera(
            float duration,
            float strength)
        {
            if (boardCamera == null)
            {
                return;
            }

            cameraShakeVersion++;

            StartCoroutine(
                AnimateCameraShake(
                    duration,
                    strength,
                    cameraShakeVersion));
        }

        private IEnumerator AnimateCameraShake(
            float duration,
            float strength,
            int version)
        {
            Vector3 basePosition =
                new Vector3(
                    0f,
                    BoardVerticalOffset,
                    -10f);

            float elapsed = 0f;

            while (elapsed < duration &&
                   version == cameraShakeVersion)
            {
                elapsed +=
                    Time.unscaledDeltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed / duration);

                float fade =
                    1f - t;

                Vector2 offset =
                    UnityEngine.Random.insideUnitCircle *
                    strength *
                    fade;

                boardCamera.transform.position =
                    basePosition +
                    new Vector3(
                        offset.x,
                        offset.y,
                        0f);

                yield return null;
            }

            if (version == cameraShakeVersion)
            {
                boardCamera.transform.position =
                    basePosition;
            }
        }

        private void DrawScreenFlash()
        {
            if (screenFlashAlpha <= 0.001f)
            {
                return;
            }

            Color oldColor =
                GUI.color;

            Color flash =
                screenFlashColor;

            flash.a =
                screenFlashAlpha;

            GUI.color = flash;

            GUI.DrawTexture(
                new Rect(
                    0f,
                    0f,
                    Screen.width,
                    Screen.height),
                Texture2D.whiteTexture,
                ScaleMode.StretchToFill);

            GUI.color = oldColor;
        }

        private bool IsScreenPointOverBoosterBar(
            Vector2 screenPosition)
        {
            float margin =
                Mathf.Max(10f, Screen.width * 0.025f);
            float barHeight =
                Mathf.Clamp(
                    Screen.height * 0.075f,
                    64f,
                    110f);

            return screenPosition.y <=
                   barHeight + (margin * 1.5f);
        }

        private void ShowBanner(
            string message,
            float duration)
        {
            transientBanner = message;
            bannerUntil =
                Time.unscaledTime + duration;
        }

        private void EnsureGuiStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            titleStyle =
                new GUIStyle(GUI.skin.label);
            titleStyle.fontStyle =
                FontStyle.Bold;
            titleStyle.alignment =
                TextAnchor.MiddleLeft;
            titleStyle.normal.textColor =
                Color.white;

            infoStyle =
                new GUIStyle(GUI.skin.label);
            infoStyle.fontStyle =
                FontStyle.Bold;
            infoStyle.alignment =
                TextAnchor.MiddleRight;
            infoStyle.normal.textColor =
                new Color(1f, 0.88f, 0.42f);

            goalStyle =
                new GUIStyle(GUI.skin.label);
            goalStyle.alignment =
                TextAnchor.MiddleLeft;
            goalStyle.normal.textColor =
                new Color(0.93f, 0.93f, 0.98f);

            statusStyle =
                new GUIStyle(GUI.skin.label);
            statusStyle.fontStyle =
                FontStyle.Bold;
            statusStyle.alignment =
                TextAnchor.MiddleCenter;
            statusStyle.normal.textColor =
                Color.white;

            buttonStyle =
                new GUIStyle(GUI.skin.button);
            buttonStyle.fontStyle =
                FontStyle.Bold;
        }

        private void OnGUI()
        {
            EnsureGuiStyles();

            if (session == null)
            {
                GUI.Box(
                    new Rect(
                        20f,
                        20f,
                        Screen.width - 40f,
                        Mathf.Min(
                            Screen.height - 40f,
                            360f)),
                    string.Empty);

                statusStyle.fontSize =
                    Mathf.RoundToInt(
                        Mathf.Clamp(
                            Screen.width * 0.040f,
                            15f,
                            24f));

                GUI.Label(
                    new Rect(
                        34f,
                        34f,
                        Screen.width - 68f,
                        Mathf.Min(
                            Screen.height - 68f,
                            330f)),
                    string.IsNullOrEmpty(
                        transientBanner)
                        ? "X MATCH STARTUP"
                        : transientBanner,
                    statusStyle);

                return;
            }

            if (showLevelSelect)
            {
                DrawLevelSelect();
                return;
            }

            int titleSize =
                Mathf.RoundToInt(
                    Mathf.Clamp(
                        Screen.width * 0.07f,
                        22f,
                        42f));
            int infoSize =
                Mathf.RoundToInt(
                    Mathf.Clamp(
                        Screen.width * 0.045f,
                        16f,
                        28f));
            int goalSize =
                Mathf.RoundToInt(
                    Mathf.Clamp(
                        Screen.width * 0.035f,
                        13f,
                        22f));

            titleStyle.fontSize = titleSize;
            infoStyle.fontSize = infoSize;
            goalStyle.fontSize = goalSize;
            statusStyle.fontSize =
                Mathf.RoundToInt(titleSize * 0.8f);
            buttonStyle.fontSize = infoSize;

            float margin =
                Mathf.Max(12f, Screen.width * 0.035f);
            float panelWidth =
                Screen.width - (margin * 2f);
            float panelHeight =
                Mathf.Clamp(
                    Screen.height * 0.13f,
                    112f,
                    190f);

            Color previousBackground =
                GUI.backgroundColor;
            GUI.backgroundColor =
                Color.Lerp(
                    ThemeColor(currentLevelIndex),
                    new Color(0.12f, 0.08f, 0.18f),
                    0.55f);

            GUI.Box(
                new Rect(
                    margin,
                    margin,
                    panelWidth,
                    panelHeight),
                string.Empty);

            GUI.backgroundColor = previousBackground;

            GUI.Label(
                new Rect(
                    margin + 16f,
                    margin + 8f,
                    panelWidth * 0.48f,
                    panelHeight * 0.42f),
                $"X MATCH  {currentLevelIndex + 1}/{PrototypeLevelFactory.LevelCount}",
                titleStyle);

            GUI.Label(
                new Rect(
                    margin + panelWidth * 0.46f,
                    margin + 8f,
                    panelWidth * 0.5f,
                    panelHeight * 0.42f),
                $"MOVES  {session.MovesRemaining}",
                infoStyle);

            var goalsText =
                new StringBuilder();

            goalsText.Append(
                PrototypeLevelFactory.GetTitle(
                    currentLevelIndex));
            goalsText.Append("  |  ");

            for (int i = 0;
                 i < session.Goals.Count;
                 i++)
            {
                if (i > 0)
                {
                    goalsText.Append("   ");
                }

                GoalProgress goal =
                    session.Goals[i];

                goalsText.Append(
                    GoalLabel(
                        goal.Definition.TileKind));
                goalsText.Append(' ');
                goalsText.Append(
                    goal.CurrentCount);
                goalsText.Append('/');
                goalsText.Append(
                    goal.Definition.TargetCount);
            }

            GUI.Label(
                new Rect(
                    margin + 16f,
                    margin + panelHeight * 0.48f,
                    panelWidth - 32f,
                    panelHeight * 0.42f),
                goalsText.ToString(),
                goalStyle);

            DrawBoosterBar();

            string status = transientBanner;

            if (session.Status == StageStatus.Won)
            {
                status = "CLEAR!";
            }
            else if (
                session.Status == StageStatus.Lost)
            {
                status = "OUT OF MOVES";
            }

            if (!string.IsNullOrEmpty(status))
            {
                float statusWidth =
                    Mathf.Min(
                        Screen.width * 0.75f,
                        460f);
                float statusHeight =
                    Mathf.Clamp(
                        Screen.height * 0.07f,
                        60f,
                        100f);

                GUI.Box(
                    new Rect(
                        (Screen.width - statusWidth) * 0.5f,
                        Screen.height * 0.47f,
                        statusWidth,
                        statusHeight),
                    string.Empty);

                GUI.Label(
                    new Rect(
                        (Screen.width - statusWidth) * 0.5f,
                        Screen.height * 0.47f,
                        statusWidth,
                        statusHeight),
                    status,
                    statusStyle);
            }

            DrawScreenFlash();

            if (session.Status !=
                StageStatus.InProgress)
            {
                float buttonWidth =
                    Mathf.Min(
                        Screen.width * 0.55f,
                        360f);
                float buttonHeight =
                    Mathf.Clamp(
                        Screen.height * 0.06f,
                        52f,
                        84f);
                float buttonX =
                    (Screen.width - buttonWidth) * 0.5f;
                float firstY =
                    Screen.height * 0.58f;

                string primary =
                    session.Status == StageStatus.Won
                        ? (currentLevelIndex + 1 <
                           PrototypeLevelFactory.LevelCount
                            ? "NEXT LEVEL"
                            : "LEVEL SELECT")
                        : "TRY AGAIN";

                Color oldBackground =
                    GUI.backgroundColor;
                GUI.backgroundColor =
                    ThemeColor(currentLevelIndex);

                if (GUI.Button(
                        new Rect(
                            buttonX,
                            firstY,
                            buttonWidth,
                            buttonHeight),
                        primary,
                        buttonStyle))
                {
                    if (session.Status == StageStatus.Won)
                    {
                        AdvanceLevel();
                    }
                    else
                    {
                        StartNewStage();
                    }
                }

                GUI.backgroundColor = oldBackground;

                if (GUI.Button(
                        new Rect(
                            buttonX,
                            firstY + buttonHeight + 10f,
                            buttonWidth,
                            buttonHeight),
                        "LEVEL SELECT",
                        buttonStyle))
                {
                    showLevelSelect = true;
                }
            }
        }

        private void DrawLevelSelect()
        {
            Color accent =
                ThemeColor(currentLevelIndex);
            Color oldBackground =
                GUI.backgroundColor;

            GUI.backgroundColor =
                Color.Lerp(
                    accent,
                    new Color(0.08f, 0.05f, 0.13f),
                    0.45f);

            float margin =
                Mathf.Max(16f, Screen.width * 0.045f);
            float width =
                Screen.width - (margin * 2f);
            float height =
                Screen.height - (margin * 2f);

            GUI.Box(
                new Rect(
                    margin,
                    margin,
                    width,
                    height),
                string.Empty);

            GUI.backgroundColor = oldBackground;

            int oldTitleSize = titleStyle.fontSize;
            int oldGoalSize = goalStyle.fontSize;
            int oldButtonSize = buttonStyle.fontSize;

            titleStyle.fontSize =
                Mathf.RoundToInt(
                    Mathf.Clamp(
                        Screen.width * 0.075f,
                        24f,
                        44f));
            titleStyle.alignment =
                TextAnchor.MiddleCenter;

            goalStyle.fontSize =
                Mathf.RoundToInt(
                    Mathf.Clamp(
                        Screen.width * 0.032f,
                        12f,
                        20f));
            goalStyle.alignment =
                TextAnchor.MiddleCenter;

            buttonStyle.fontSize =
                Mathf.RoundToInt(
                    Mathf.Clamp(
                        Screen.width * 0.030f,
                        11f,
                        19f));

            GUI.Label(
                new Rect(
                    margin + 12f,
                    margin + 12f,
                    width - 24f,
                    58f),
                "X MATCH  ·  10 STAGES",
                titleStyle);

            GUI.Label(
                new Rect(
                    margin + 12f,
                    margin + 62f,
                    width - 24f,
                    42f),
                "SPECIAL BLOCK LAB · BOOSTERS ∞",
                goalStyle);

            float gridTop =
                margin + 116f;
            float gap =
                Mathf.Max(8f, Screen.width * 0.018f);
            float buttonWidth =
                (width - 36f - gap) * 0.5f;
            float availableHeight =
                height - 142f;
            float buttonHeight =
                (availableHeight - (gap * 4f)) / 5f;

            for (int i = 0;
                 i < PrototypeLevelFactory.LevelCount;
                 i++)
            {
                int column = i % 2;
                int row = i / 2;

                Rect rect =
                    new Rect(
                        margin + 18f +
                        (column * (buttonWidth + gap)),
                        gridTop +
                        (row * (buttonHeight + gap)),
                        buttonWidth,
                        buttonHeight);

                GUI.backgroundColor =
                    i == currentLevelIndex
                        ? ThemeColor(i)
                        : Color.Lerp(
                            ThemeColor(i),
                            Color.gray,
                            0.45f);

                string label =
                    $"{i + 1:00}  " +
                    PrototypeLevelFactory.GetTitle(i) +
                    "\n" +
                    PrototypeLevelFactory.GetHint(i);

                if (GUI.Button(
                        rect,
                        label,
                        buttonStyle))
                {
                    showLevelSelect = false;
                    StartLevel(i);
                }
            }

            GUI.backgroundColor = oldBackground;
            titleStyle.fontSize = oldTitleSize;
            titleStyle.alignment =
                TextAnchor.MiddleLeft;
            goalStyle.fontSize = oldGoalSize;
            goalStyle.alignment =
                TextAnchor.MiddleLeft;
            buttonStyle.fontSize = oldButtonSize;
        }

        private void DrawBoosterBar()
        {
            if (session.Status != StageStatus.InProgress ||
                inputLocked)
            {
                return;
            }

            float margin =
                Mathf.Max(10f, Screen.width * 0.025f);
            float barHeight =
                Mathf.Clamp(
                    Screen.height * 0.075f,
                    64f,
                    110f);
            float y =
                Screen.height - barHeight - margin;
            float gap =
                Mathf.Max(5f, Screen.width * 0.012f);
            float totalWidth =
                Screen.width - (margin * 2f);
            float buttonWidth =
                (totalWidth - (gap * 3f)) / 4f;

            int previousFontSize = buttonStyle.fontSize;
            buttonStyle.fontSize =
                Mathf.RoundToInt(
                    Mathf.Clamp(
                        Screen.width * 0.026f,
                        10f,
                        18f));

            DrawBoosterButton(
                BoosterKind.Hammer,
                "HAMMER ∞",
                new Rect(
                    margin,
                    y,
                    buttonWidth,
                    barHeight));

            DrawBoosterButton(
                BoosterKind.RowClear,
                "ROW ∞",
                new Rect(
                    margin + buttonWidth + gap,
                    y,
                    buttonWidth,
                    barHeight));

            DrawBoosterButton(
                BoosterKind.ColumnClear,
                "COL ∞",
                new Rect(
                    margin + ((buttonWidth + gap) * 2f),
                    y,
                    buttonWidth,
                    barHeight));

            if (GUI.Button(
                    new Rect(
                        margin + ((buttonWidth + gap) * 3f),
                        y,
                        buttonWidth,
                        barHeight),
                    "SHUFFLE ∞",
                    buttonStyle))
            {
                selectedBooster = BoosterKind.None;

                if (session.UseShuffleBooster())
                {
                    if (audioDirector != null)
                    {
                        audioDirector.Play(
                            XMatchSoundKind.Shuffle,
                            0.78f);
                    }

                    BoardPosition center =
                        new BoardPosition(
                            session.Board.Width / 2,
                            session.Board.Height / 2);

                    PlayVfx(
                        XMatchVfxKind.MagicCircle,
                        center,
                        0.48f,
                        new Vector3(
                            3.8f,
                            3.8f,
                            1f));

                    FlashScreen(
                        ThemeColor(currentLevelIndex),
                        0.22f,
                        0.18f);

                    ClearVisuals();
                    BuildVisuals();
                    ShowBanner("SHUFFLE ∞", 0.8f);
                }
                else
                {
                    ShowBanner("SHUFFLE FAILED", 0.8f);
                }
            }

            buttonStyle.fontSize = previousFontSize;
        }

        private void DrawBoosterButton(
            BoosterKind booster,
            string label,
            Rect rect)
        {
            string text =
                selectedBooster == booster
                    ? "> " + label
                    : label;

            if (!GUI.Button(rect, text, buttonStyle))
            {
                return;
            }

            if (selectedBooster == booster)
            {
                selectedBooster = BoosterKind.None;
                ShowBanner("BOOSTER OFF", 0.5f);
                return;
            }

            selectedBooster = booster;
            ShowBanner(
                BoosterLabel(booster) + " : TAP TILE",
                1.0f);
        }

        private static string BoosterLabel(
            BoosterKind booster)
        {
            switch (booster)
            {
                case BoosterKind.Hammer:
                    return "HAMMER ∞";
                case BoosterKind.RowClear:
                    return "ROW CLEAR ∞";
                case BoosterKind.ColumnClear:
                    return "COLUMN CLEAR ∞";
                case BoosterKind.Shuffle:
                    return "SHUFFLE ∞";
                default:
                    return "BOOSTER";
            }
        }

        private static string PowerUpLabel(
            PowerUpKind powerUp)
        {
            switch (powerUp)
            {
                case PowerUpKind.RowBlast:
                    return "ROW BLAST!";
                case PowerUpKind.ColumnBlast:
                    return "COLUMN BLAST!";
                case PowerUpKind.Bomb:
                    return "BOMB!";
                case PowerUpKind.ColorOrb:
                    return "COLOR ORB!";
                case PowerUpKind.Seeker:
                    return "SEEKER!";
                default:
                    return string.Empty;
            }
        }

        private static Color ThemeColor(
            int levelIndex)
        {
            switch (levelIndex)
            {
                case 0:
                    return new Color(0.98f, 0.22f, 0.43f);
                case 1:
                    return new Color(0.78f, 0.20f, 0.62f);
                case 2:
                    return new Color(0.20f, 0.68f, 1.00f);
                case 3:
                    return new Color(1.00f, 0.48f, 0.18f);
                case 4:
                    return new Color(0.55f, 0.30f, 1.00f);
                case 5:
                    return new Color(1.00f, 0.22f, 0.18f);
                case 6:
                    return new Color(0.22f, 0.88f, 0.54f);
                case 7:
                    return new Color(0.24f, 0.78f, 0.92f);
                case 8:
                    return new Color(0.92f, 0.28f, 0.82f);
                case 9:
                    return new Color(1.00f, 0.72f, 0.18f);
                default:
                    return new Color(0.78f, 0.28f, 0.82f);
            }
        }

        private static string GoalLabel(
            TileKind kind)
        {
            switch (kind)
            {
                case TileKind.Heart:
                    return "HEART";
                case TileKind.Lips:
                    return "LIPS";
                case TileKind.Diamond:
                    return "DIAMOND";
                case TileKind.Perfume:
                    return "PERFUME";
                case TileKind.Rose:
                    return "ROSE";
                default:
                    return kind.ToString().ToUpperInvariant();
            }
        }

        private static float Smooth01(float t)
        {
            return t * t * (3f - (2f * t));
        }

        private readonly struct MoveAnimation
        {
            public MoveAnimation(
                TileView view,
                Vector3 start,
                Vector3 target,
                BoardPosition targetCell)
            {
                View = view;
                Start = start;
                Target = target;
                TargetCell = targetCell;
            }

            public TileView View { get; }
            public Vector3 Start { get; }
            public Vector3 Target { get; }
            public BoardPosition TargetCell { get; }
        }
    }
}
