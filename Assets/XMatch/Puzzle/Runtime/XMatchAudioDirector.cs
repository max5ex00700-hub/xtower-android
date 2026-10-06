using System;
using System.Collections.Generic;
using UnityEngine;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public enum XMatchSoundKind
    {
        Tap = 0,
        Swap = 1,
        Invalid = 2,
        Match = 3,
        CreateSpecial = 4,
        RowBlast = 5,
        ColumnBlast = 6,
        Bomb = 7,
        ColorOrb = 8,
        Seeker = 9,
        Booster = 10,
        Shuffle = 11,
        Clear = 12,
        Fail = 13,
        Heart = 14,
        Lips = 15,
        Diamond = 16,
        Perfume = 17,
        Rose = 18,
        ComboCross = 19,
        ComboBomb = 20,
        ComboOrb = 21,
        Gift = 22,
        Wand = 23
    }

    public sealed class XMatchAudioDirector : MonoBehaviour
    {
        private const int SampleRate = 22050;
        private const int SourceCount = 5;

        private readonly Dictionary<XMatchSoundKind, AudioClip>
            clips =
                new Dictionary<XMatchSoundKind, AudioClip>();

        private AudioSource[] sources;
        private int sourceIndex;

        private void Awake()
        {
            sources = new AudioSource[SourceCount];

            for (int i = 0; i < SourceCount; i++)
            {
                AudioSource source =
                    gameObject.AddComponent<AudioSource>();

                source.playOnAwake = false;
                source.loop = false;
                source.spatialBlend = 0f;
                source.volume = 1f;
                source.ignoreListenerPause = true;

                sources[i] = source;
            }

            BuildClips();
        }

        private void OnDestroy()
        {
            foreach (
                KeyValuePair<XMatchSoundKind, AudioClip> pair
                in clips)
            {
                if (pair.Value != null)
                {
                    Destroy(pair.Value);
                }
            }

            clips.Clear();
        }

        public void Play(
            XMatchSoundKind kind,
            float volume = 1f,
            float pitch = 1f)
        {
            AudioClip clip;

            if (!clips.TryGetValue(kind, out clip) ||
                clip == null ||
                sources == null ||
                sources.Length == 0)
            {
                return;
            }

            AudioSource source =
                sources[sourceIndex];

            sourceIndex =
                (sourceIndex + 1) %
                sources.Length;

            source.Stop();
            source.pitch =
                Mathf.Clamp(pitch, 0.72f, 1.45f);
            source.volume =
                Mathf.Clamp01(volume);
            source.clip = clip;
            source.Play();
        }

        public void PlayMatch(int chainNumber)
        {
            float pitch =
                1f +
                (Mathf.Clamp(chainNumber - 1, 0, 6) *
                 0.045f);

            float volume =
                chainNumber <= 1
                    ? 0.62f
                    : 0.72f;

            Play(
                XMatchSoundKind.Match,
                volume,
                pitch);
        }

        public void PlayTile(
            TileKind kind,
            int chainNumber)
        {
            XMatchSoundKind sound;

            switch (kind)
            {
                case TileKind.Heart:
                    sound = XMatchSoundKind.Heart;
                    break;
                case TileKind.Lips:
                    sound = XMatchSoundKind.Lips;
                    break;
                case TileKind.Diamond:
                    sound = XMatchSoundKind.Diamond;
                    break;
                case TileKind.Perfume:
                    sound = XMatchSoundKind.Perfume;
                    break;
                case TileKind.Rose:
                    sound = XMatchSoundKind.Rose;
                    break;
                default:
                    PlayMatch(chainNumber);
                    return;
            }

            float pitch =
                1f +
                (Mathf.Clamp(
                    chainNumber - 1,
                    0,
                    6) *
                 0.035f);

            Play(
                sound,
                chainNumber > 1
                    ? 0.74f
                    : 0.62f,
                pitch);
        }

        public void PlaySpecialCombo(
            SpecialComboKind combo)
        {
            switch (combo)
            {
                case SpecialComboKind.CrossBlast:
                case SpecialComboKind.DoubleRow:
                case SpecialComboKind.DoubleColumn:
                case SpecialComboKind.RowBomb:
                case SpecialComboKind.ColumnBomb:
                    Play(
                        XMatchSoundKind.ComboCross,
                        0.95f);
                    break;

                case SpecialComboKind.DoubleBomb:
                case SpecialComboKind.SeekerPair:
                case SpecialComboKind.SeekerWithSpecial:
                    Play(
                        XMatchSoundKind.ComboBomb,
                        1f);
                    break;

                case SpecialComboKind.OrbRow:
                case SpecialComboKind.OrbColumn:
                case SpecialComboKind.OrbBomb:
                case SpecialComboKind.OrbSeeker:
                case SpecialComboKind.DoubleOrb:
                    Play(
                        XMatchSoundKind.ComboOrb,
                        1f);
                    break;
            }
        }

        private void BuildClips()
        {
            clips[XMatchSoundKind.Tap] =
                CreateSweep(
                    "Tap",
                    0.055f,
                    880f,
                    1260f,
                    0.03f,
                    0.10f,
                    0.06f,
                    0.35f,
                    1);

            clips[XMatchSoundKind.Swap] =
                CreateSweep(
                    "Swap",
                    0.095f,
                    420f,
                    760f,
                    0.04f,
                    0.18f,
                    0.08f,
                    0.38f,
                    2);

            clips[XMatchSoundKind.Invalid] =
                CreateSweep(
                    "Invalid",
                    0.13f,
                    240f,
                    120f,
                    0.12f,
                    0.28f,
                    0.04f,
                    0.40f,
                    3);

            clips[XMatchSoundKind.Match] =
                CreateChord(
                    "Match",
                    0.18f,
                    740f,
                    930f,
                    1175f,
                    0.055f,
                    0.48f,
                    4);

            clips[XMatchSoundKind.CreateSpecial] =
                CreateChord(
                    "CreateSpecial",
                    0.34f,
                    880f,
                    1175f,
                    1568f,
                    0.08f,
                    0.52f,
                    5);

            clips[XMatchSoundKind.RowBlast] =
                CreateBlast(
                    "RowBlast",
                    0.34f,
                    260f,
                    1180f,
                    0.32f,
                    6);

            clips[XMatchSoundKind.ColumnBlast] =
                CreateBlast(
                    "ColumnBlast",
                    0.36f,
                    320f,
                    1450f,
                    0.28f,
                    7);

            clips[XMatchSoundKind.Bomb] =
                CreateExplosion(
                    "Bomb",
                    0.46f,
                    86f,
                    0.72f,
                    8);

            clips[XMatchSoundKind.ColorOrb] =
                CreateColorOrb(
                    "ColorOrb",
                    0.72f,
                    9);

            clips[XMatchSoundKind.Seeker] =
                CreateSweep(
                    "Seeker",
                    0.28f,
                    520f,
                    1680f,
                    0.10f,
                    0.18f,
                    0.08f,
                    0.48f,
                    10);

            clips[XMatchSoundKind.Booster] =
                CreateChord(
                    "Booster",
                    0.20f,
                    520f,
                    780f,
                    1040f,
                    0.06f,
                    0.46f,
                    11);

            clips[XMatchSoundKind.Shuffle] =
                CreateBlast(
                    "Shuffle",
                    0.38f,
                    180f,
                    920f,
                    0.48f,
                    12);

            clips[XMatchSoundKind.Clear] =
                CreateChord(
                    "Clear",
                    0.82f,
                    523.25f,
                    659.25f,
                    783.99f,
                    0.035f,
                    0.58f,
                    13);

            clips[XMatchSoundKind.Fail] =
                CreateSweep(
                    "Fail",
                    0.72f,
                    330f,
                    118f,
                    0.04f,
                    0.30f,
                    0.03f,
                    0.48f,
                    14);

            clips[XMatchSoundKind.Heart] =
                CreateChord(
                    "Heart",
                    0.22f,
                    523.25f,
                    783.99f,
                    1046.5f,
                    0.018f,
                    0.40f,
                    21);

            clips[XMatchSoundKind.Lips] =
                CreateSweep(
                    "LipsKiss",
                    0.17f,
                    280f,
                    760f,
                    0.14f,
                    0.36f,
                    0.008f,
                    0.46f,
                    22);

            clips[XMatchSoundKind.Diamond] =
                CreateChord(
                    "Diamond",
                    0.24f,
                    1046.5f,
                    1318.5f,
                    1568f,
                    0.018f,
                    0.42f,
                    23);

            clips[XMatchSoundKind.Perfume] =
                CreateSweep(
                    "PerfumeSpray",
                    0.28f,
                    1580f,
                    620f,
                    0.25f,
                    0.06f,
                    0.025f,
                    0.34f,
                    24);

            clips[XMatchSoundKind.Rose] =
                CreateSweep(
                    "RosePetal",
                    0.25f,
                    420f,
                    820f,
                    0.07f,
                    0.15f,
                    0.035f,
                    0.34f,
                    25);

            clips[XMatchSoundKind.ComboCross] =
                CreateBlast(
                    "ComboCross",
                    0.46f,
                    300f,
                    1760f,
                    0.23f,
                    26);

            clips[XMatchSoundKind.ComboBomb] =
                CreateExplosion(
                    "ComboBomb",
                    0.62f,
                    72f,
                    0.78f,
                    27);

            clips[XMatchSoundKind.ComboOrb] =
                CreateColorOrb(
                    "ComboOrb",
                    0.96f,
                    28);

            clips[XMatchSoundKind.Gift] =
                CreateChord(
                    "Gift",
                    0.52f,
                    659.25f,
                    987.77f,
                    1318.5f,
                    0.025f,
                    0.52f,
                    29);

            clips[XMatchSoundKind.Wand] =
                CreateSweep(
                    "Wand",
                    0.48f,
                    520f,
                    1960f,
                    0.05f,
                    0.18f,
                    0.025f,
                    0.48f,
                    30);
        }

        private static AudioClip CreateSweep(
            string name,
            float duration,
            float startFrequency,
            float endFrequency,
            float noiseAmount,
            float harmonicAmount,
            float attack,
            float gain,
            int seed)
        {
            int count =
                Mathf.Max(
                    16,
                    Mathf.CeilToInt(
                        duration * SampleRate));

            var samples =
                new float[count];

            var random =
                new System.Random(seed);

            double phase = 0.0;

            for (int i = 0; i < count; i++)
            {
                float t =
                    i / (float)(count - 1);

                float frequency =
                    Mathf.Lerp(
                        startFrequency,
                        endFrequency,
                        Smooth01(t));

                phase +=
                    (Math.PI * 2.0 *
                     frequency) /
                    SampleRate;

                float sine =
                    Mathf.Sin((float)phase);

                float harmonic =
                    Mathf.Sin(
                        (float)phase * 2f) *
                    harmonicAmount;

                float noise =
                    (((float)random.NextDouble() * 2f) - 1f) *
                    noiseAmount;

                float envelope =
                    Envelope(
                        t,
                        attack,
                        0.28f);

                samples[i] =
                    Mathf.Clamp(
                        (sine + harmonic + noise) *
                        envelope *
                        gain,
                        -1f,
                        1f);
            }

            return BuildClip(
                name,
                samples);
        }

        private static AudioClip CreateChord(
            string name,
            float duration,
            float first,
            float second,
            float third,
            float noiseAmount,
            float gain,
            int seed)
        {
            int count =
                Mathf.Max(
                    16,
                    Mathf.CeilToInt(
                        duration * SampleRate));

            var samples =
                new float[count];

            var random =
                new System.Random(seed);

            double p1 = 0.0;
            double p2 = 0.0;
            double p3 = 0.0;

            for (int i = 0; i < count; i++)
            {
                float t =
                    i / (float)(count - 1);

                float shimmer =
                    1f +
                    (Mathf.Sin(t * Mathf.PI * 5f) *
                     0.012f);

                p1 +=
                    (Math.PI * 2.0 *
                     first *
                     shimmer) /
                    SampleRate;
                p2 +=
                    (Math.PI * 2.0 *
                     second *
                     shimmer) /
                    SampleRate;
                p3 +=
                    (Math.PI * 2.0 *
                     third *
                     shimmer) /
                    SampleRate;

                float chord =
                    (Mathf.Sin((float)p1) * 0.50f) +
                    (Mathf.Sin((float)p2) * 0.32f) +
                    (Mathf.Sin((float)p3) * 0.24f);

                float sparkle =
                    (((float)random.NextDouble() * 2f) - 1f) *
                    noiseAmount *
                    (1f - t);

                float envelope =
                    Envelope(
                        t,
                        0.035f,
                        0.42f);

                samples[i] =
                    Mathf.Clamp(
                        (chord + sparkle) *
                        envelope *
                        gain,
                        -1f,
                        1f);
            }

            return BuildClip(
                name,
                samples);
        }

        private static AudioClip CreateBlast(
            string name,
            float duration,
            float startFrequency,
            float endFrequency,
            float noiseAmount,
            int seed)
        {
            int count =
                Mathf.Max(
                    16,
                    Mathf.CeilToInt(
                        duration * SampleRate));

            var samples =
                new float[count];

            var random =
                new System.Random(seed);

            double phase = 0.0;

            for (int i = 0; i < count; i++)
            {
                float t =
                    i / (float)(count - 1);

                float frequency =
                    Mathf.Lerp(
                        startFrequency,
                        endFrequency,
                        t);

                phase +=
                    (Math.PI * 2.0 *
                     frequency) /
                    SampleRate;

                float sweep =
                    Mathf.Sin((float)phase) *
                    0.42f;

                float noise =
                    (((float)random.NextDouble() * 2f) - 1f) *
                    noiseAmount;

                float click =
                    Mathf.Sin(
                        (float)phase * 3f) *
                    0.14f;

                float envelope =
                    Envelope(
                        t,
                        0.015f,
                        0.40f);

                samples[i] =
                    Mathf.Clamp(
                        (sweep + noise + click) *
                        envelope *
                        0.62f,
                        -1f,
                        1f);
            }

            return BuildClip(
                name,
                samples);
        }

        private static AudioClip CreateExplosion(
            string name,
            float duration,
            float baseFrequency,
            float noiseAmount,
            int seed)
        {
            int count =
                Mathf.Max(
                    16,
                    Mathf.CeilToInt(
                        duration * SampleRate));

            var samples =
                new float[count];

            var random =
                new System.Random(seed);

            double phase = 0.0;

            for (int i = 0; i < count; i++)
            {
                float t =
                    i / (float)(count - 1);

                float frequency =
                    Mathf.Lerp(
                        baseFrequency,
                        baseFrequency * 0.48f,
                        t);

                phase +=
                    (Math.PI * 2.0 *
                     frequency) /
                    SampleRate;

                float low =
                    Mathf.Sin((float)phase) *
                    0.72f;

                float crack =
                    (((float)random.NextDouble() * 2f) - 1f) *
                    noiseAmount *
                    Mathf.Pow(1f - t, 1.7f);

                float envelope =
                    Envelope(
                        t,
                        0.006f,
                        0.58f);

                samples[i] =
                    Mathf.Clamp(
                        (low + crack) *
                        envelope *
                        0.78f,
                        -1f,
                        1f);
            }

            return BuildClip(
                name,
                samples);
        }

        private static AudioClip CreateColorOrb(
            string name,
            float duration,
            int seed)
        {
            int count =
                Mathf.Max(
                    16,
                    Mathf.CeilToInt(
                        duration * SampleRate));

            var samples =
                new float[count];

            var random =
                new System.Random(seed);

            double phase = 0.0;
            double bellPhase = 0.0;

            for (int i = 0; i < count; i++)
            {
                float t =
                    i / (float)(count - 1);

                float charge =
                    Mathf.Clamp01(t / 0.56f);

                float frequency =
                    Mathf.Lerp(
                        240f,
                        1880f,
                        charge * charge);

                phase +=
                    (Math.PI * 2.0 *
                     frequency) /
                    SampleRate;

                bellPhase +=
                    (Math.PI * 2.0 *
                     1046.5f) /
                    SampleRate;

                float sweep =
                    Mathf.Sin((float)phase) *
                    0.42f;

                float bell =
                    Mathf.Sin((float)bellPhase) *
                    Mathf.Clamp01(
                        (t - 0.46f) * 4f) *
                    (1f - t) *
                    0.36f;

                float glitter =
                    (((float)random.NextDouble() * 2f) - 1f) *
                    0.09f *
                    (1f - t);

                float envelope =
                    Envelope(
                        t,
                        0.03f,
                        0.48f);

                samples[i] =
                    Mathf.Clamp(
                        (sweep + bell + glitter) *
                        envelope *
                        0.68f,
                        -1f,
                        1f);
            }

            return BuildClip(
                name,
                samples);
        }

        private static AudioClip BuildClip(
            string name,
            float[] samples)
        {
            AudioClip clip =
                AudioClip.Create(
                    "XMatch_" + name,
                    samples.Length,
                    1,
                    SampleRate,
                    false);

            clip.SetData(
                samples,
                0);

            return clip;
        }

        private static float Envelope(
            float t,
            float attack,
            float release)
        {
            float attackValue =
                attack <= 0f
                    ? 1f
                    : Mathf.Clamp01(t / attack);

            float releaseValue =
                release <= 0f
                    ? 1f
                    : Mathf.Clamp01(
                        (1f - t) / release);

            return
                attackValue *
                releaseValue *
                releaseValue;
        }

        private static float Smooth01(float t)
        {
            return
                t *
                t *
                (3f - (2f * t));
        }
    }
}
