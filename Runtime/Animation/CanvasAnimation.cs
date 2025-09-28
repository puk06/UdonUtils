using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using net.puk06.Utils;

namespace net.puk06.Canvas.Animation
{
    public enum TransitionType
    {
        None,
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }

    public enum AnimationMode
    {
        None,
        FadeIn,
        FadeOut,
        MoveDown,
        MoveUp,
        MoveLeft,
        MoveRight,
        Toggle
    }

    public enum ElementType
    {
        None,
        Text,
        Button,
        DropDown,
        InputField,
        Image,
        RawImage,
        Toggle,
        Slider,
        Scrollbar,

        TMP_Text,
        TMP_Dropdown,
        TMP_InputField
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CanvasAnimation : UdonSharpBehaviour
    {
        [Header("同時実行できるアニメーション数（多すぎると重くなります）")]
        [Tooltip("一度に再生できるアニメーションの最大数を設定します。\nおすすめは「これまでの同時実行アニメーション最大数」か「その+1」です。")]
        [SerializeField] private int maxConcurrentAnimations = 32;

        #region 確認用
        [Header("現在実行中のアニメーション数（確認用）")]
        [Tooltip("現在進行中のアニメーション数を表示します。終了したアニメーションはカウントされません。")]
        [SerializeField] private int runningAnimations = 0;

        [Header("これまでの同時実行アニメーション最大数（確認用）")]
        [Tooltip("今まで同時に再生されたアニメーション数の最大値を記録します。\nMaxConcurrentAnimationsを決める際の目安にしてください。")]
        [SerializeField] private int peakConcurrentAnimations = 0;
        #endregion

        private string[] m_currentTasks;

        private Text[] m_targetTexts;
        private Button[] m_targetButtons;
        // private Dropdown[] targetDropDowns;
        // private InputField[] targetInputFields;
        private Image[] m_targetImages;
        private RawImage[] m_targetRawImages;
        // private Toggle[] targetToggles;
        // private Slider[] targetSliders;
        // private Scrollbar[] targetScrollbars;

        private TMP_Text[] m_targetTMPTexts;
        // private TMP_Dropdown[] targetTMPDropDowns;
        // private TMP_InputField[] targetTMPInputFields;

        private float[] m_durations;
        private int[] m_pixelOffsets;
        private float[] m_startTimes;
        private float[] m_timeoutTimes;
        private Vector3[] m_startLocations;
        private TransitionType[] m_transitions;
        private ElementType[] m_elementTypes;
        private AnimationMode[] m_modes;

        void Start()
        {
            m_currentTasks = new string[maxConcurrentAnimations];

            m_targetTexts = new Text[maxConcurrentAnimations];
            m_targetButtons = new Button[maxConcurrentAnimations];
            m_targetImages = new Image[maxConcurrentAnimations];
            m_targetRawImages = new RawImage[maxConcurrentAnimations];
            m_targetTMPTexts = new TMP_Text[maxConcurrentAnimations];

            m_durations = new float[maxConcurrentAnimations];
            m_pixelOffsets = new int[maxConcurrentAnimations];
            m_startTimes = new float[maxConcurrentAnimations];
            m_timeoutTimes = new float[maxConcurrentAnimations];
            m_startLocations = new Vector3[maxConcurrentAnimations];
            m_transitions = new TransitionType[maxConcurrentAnimations];
            m_elementTypes = new ElementType[maxConcurrentAnimations];
            m_modes = new AnimationMode[maxConcurrentAnimations];

            InitializeValues(m_durations);
            InitializeValues(m_pixelOffsets);
            InitializeValues(m_startTimes);
            InitializeValues(m_timeoutTimes);
            InitializeValues(m_startLocations);
        }

        private void InitializeValues(float[] floats)
        {
            for (int i = 0; i < floats.Length; i++)
            {
                floats[i] = -1;
            }
        }

        private void InitializeValues(int[] floats)
        {
            for (int i = 0; i < floats.Length; i++)
            {
                floats[i] = -1;
            }
        }

        private void InitializeValues(Vector3[] vectors)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = Vector3.positiveInfinity;
            }
        }

        /// <summary>
        /// Gradually increases the target’s opacity from transparent to fully visible.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeIn(Text text, float time, float after, TransitionType transition)
        {
            AddTask(text, time, after, 0, transition, AnimationMode.FadeIn, ElementType.Text);
            return this;
        }

        /// <summary>
        /// Gradually increases the target’s opacity from transparent to fully visible.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeIn(Button button, float time, float after, TransitionType transition)
        {
            AddTask(button, time, after, 0, transition, AnimationMode.FadeIn, ElementType.Button);
            return this;
        }

        /// <summary>
        /// Gradually increases the target’s opacity from transparent to fully visible.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeIn(Image image, float time, float after, TransitionType transition)
        {
            AddTask(image, time, after, 0, transition, AnimationMode.FadeIn, ElementType.Image);
            return this;
        }

        /// <summary>
        /// Gradually increases the target’s opacity from transparent to fully visible.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeIn(RawImage rawImage, float time, float after, TransitionType transition)
        {
            AddTask(rawImage, time, after, 0, transition, AnimationMode.FadeIn, ElementType.RawImage);
            return this;
        }

        /// <summary>
        /// Gradually increases the target’s opacity from transparent to fully visible.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeIn(TMP_Text text, float time, float after, TransitionType transition)
        {
            AddTask(text, time, after, 0, transition, AnimationMode.FadeIn, ElementType.TMP_Text);
            return this;
        }

        /// <summary>
        /// Gradually decreases the target’s opacity from fully visible to transparent.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeOut(Text text, float time, float after, TransitionType transition)
        {
            AddTask(text, time, after, 0, transition, AnimationMode.FadeOut, ElementType.Text);
            return this;
        }

        /// <summary>
        /// Gradually decreases the target’s opacity from fully visible to transparent.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeOut(Button button, float time, float after, TransitionType transition)
        {
            AddTask(button, time, after, 0, transition, AnimationMode.FadeOut, ElementType.Button);
            return this;
        }

        /// <summary>
        /// Gradually decreases the target’s opacity from fully visible to transparent.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeOut(Image image, float time, float after, TransitionType transition)
        {
            AddTask(image, time, after, 0, transition, AnimationMode.FadeOut, ElementType.Image);
            return this;
        }

        /// <summary>
        /// Gradually decreases the target’s opacity from fully visible to transparent.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeOut(RawImage rawImage, float time, float after, TransitionType transition)
        {
            AddTask(rawImage, time, after, 0, transition, AnimationMode.FadeOut, ElementType.RawImage);
            return this;
        }

        /// <summary>
        /// Gradually decreases the target’s opacity from fully visible to transparent.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation FadeOut(TMP_Text text, float time, float after, TransitionType transition)
        {
            AddTask(text, time, after, 0, transition, AnimationMode.FadeOut, ElementType.TMP_Text);
            return this;
        }

        /// <summary>
        /// Moves the target downward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveDown(Text text, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(text, time, after, pixelOffset, transition, AnimationMode.MoveDown, ElementType.Text);
            return this;
        }

        /// <summary>
        /// Moves the target downward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveDown(Button button, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(button, time, after, pixelOffset, transition, AnimationMode.MoveDown, ElementType.Button);
            return this;
        }

        /// <summary>
        /// Moves the target downward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveDown(Image image, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(image, time, after, pixelOffset, transition, AnimationMode.MoveDown, ElementType.Image);
            return this;
        }

        /// <summary>
        /// Moves the target downward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveDown(RawImage rawImage, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(rawImage, time, after, pixelOffset, transition, AnimationMode.MoveDown, ElementType.RawImage);
            return this;
        }

        /// <summary>
        /// Moves the target downward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveDown(TMP_Text text, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(text, time, after, pixelOffset, transition, AnimationMode.MoveDown, ElementType.TMP_Text);
            return this;
        }

        /// <summary>
        /// Moves the target upward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveUp(Text text, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(text, time, after, pixelOffset, transition, AnimationMode.MoveUp, ElementType.Text);
            return this;
        }

        /// <summary>
        /// Moves the target upward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveUp(Button button, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(button, time, after, pixelOffset, transition, AnimationMode.MoveUp, ElementType.Button);
            return this;
        }

        /// <summary>
        /// Moves the target upward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveUp(Image image, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(image, time, after, pixelOffset, transition, AnimationMode.MoveUp, ElementType.Image);
            return this;
        }

        /// <summary>
        /// Moves the target upward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveUp(RawImage rawImage, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(rawImage, time, after, pixelOffset, transition, AnimationMode.MoveUp, ElementType.RawImage);
            return this;
        }

        /// <summary>
        /// Moves the target upward by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveUp(TMP_Text text, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(text, time, after, pixelOffset, transition, AnimationMode.MoveUp, ElementType.TMP_Text);
            return this;
        }

        /// <summary>
        /// Moves the target to the left by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveLeft(Text text, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(text, time, after, pixelOffset, transition, AnimationMode.MoveLeft, ElementType.Text);
            return this;
        }

        /// <summary>
        /// Moves the target to the left by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveLeft(Button button, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(button, time, after, pixelOffset, transition, AnimationMode.MoveLeft, ElementType.Button);
            return this;
        }

        /// <summary>
        /// Moves the target to the left by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveLeft(Image image, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(image, time, after, pixelOffset, transition, AnimationMode.MoveLeft, ElementType.Image);
            return this;
        }

        /// <summary>
        /// Moves the target to the left by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveLeft(RawImage rawImage, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(rawImage, time, after, pixelOffset, transition, AnimationMode.MoveLeft, ElementType.RawImage);
            return this;
        }

        /// <summary>
        /// Moves the target to the left by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveLeft(TMP_Text text, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(text, time, after, pixelOffset, transition, AnimationMode.MoveLeft, ElementType.TMP_Text);
            return this;
        }

        /// <summary>
        /// Moves the target to the right by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveRight(Text text, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(text, time, after, pixelOffset, transition, AnimationMode.MoveRight, ElementType.Text);
            return this;
        }

        /// <summary>
        /// Moves the target to the right by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveRight(Button button, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(button, time, after, pixelOffset, transition, AnimationMode.MoveRight, ElementType.Button);
            return this;
        }

        /// <summary>
        /// Moves the target to the right by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveRight(Image image, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(image, time, after, pixelOffset, transition, AnimationMode.MoveRight, ElementType.Image);
            return this;
        }

        /// <summary>
        /// Moves the target to the right by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveRight(RawImage rawImage, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(rawImage, time, after, pixelOffset, transition, AnimationMode.MoveRight, ElementType.RawImage);
            return this;
        }

        /// <summary>
        /// Moves the target to the right by the specified distance over the given duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="after"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CanvasAnimation MoveRight(TMP_Text text, float time, float after, int pixelOffset, TransitionType transition)
        {
            AddTask(text, time, after, pixelOffset, transition, AnimationMode.MoveRight, ElementType.TMP_Text);
            return this;
        }

        private void AddTask(object element, float time, float after, int pixelOffset, TransitionType transition, AnimationMode mode, ElementType elementType)
        {
            string stats;

            switch (elementType)
            {
                case ElementType.Text:
                    {
                        Text text = (Text)element;
                        int objectIndex = AssignArrayValue(m_targetTexts, text);
                        stats = AssignData(objectIndex, text.gameObject, time, after, pixelOffset, transition, mode, elementType);
                        break;
                    }
                case ElementType.Button:
                    {
                        Button button = (Button)element;
                        int objectIndex = AssignArrayValue(m_targetButtons, button);
                        stats = AssignData(objectIndex, button.gameObject, time, after, pixelOffset, transition, mode, elementType);
                        break;
                    }
                case ElementType.Image:
                    {
                        Image image = (Image)element;
                        int objectIndex = AssignArrayValue(m_targetImages, image);
                        stats = AssignData(objectIndex, image.gameObject, time, after, pixelOffset, transition, mode, elementType);
                        break;
                    }
                case ElementType.RawImage:
                    {
                        RawImage rawImage = (RawImage)element;
                        int objectIndex = AssignArrayValue(m_targetRawImages, rawImage);
                        stats = AssignData(objectIndex, rawImage.gameObject, time, after, pixelOffset, transition, mode, elementType);
                        break;
                    }
                case ElementType.TMP_Text:
                    {
                        TMP_Text tmpText = (TMP_Text)element;
                        int objectIndex = AssignArrayValue(m_targetTMPTexts, tmpText);
                        stats = AssignData(objectIndex, tmpText.gameObject, time, after, pixelOffset, transition, mode, elementType);
                        break;
                    }
                default:
                    return;
            }

            int statsIndex = AssignArrayValue(m_currentTasks, stats);
            if (statsIndex == -1)
            {
                Debug.LogError("Couldn't Assign Task Data.");
            }
        }

        private string AssignData(int objectIndex, GameObject gameObject, float time, float after, int pixelOffset, TransitionType transition, AnimationMode mode, ElementType elementType)
        {
            int durationIndex = AssignArrayValue(m_durations, time);
            int startTimeIndex = AssignArrayValue(m_startTimes, Time.time);
            int pixelOffsetIndex = AssignArrayValue(m_pixelOffsets, pixelOffset);
            int timeoutTimesIndex = AssignArrayValue(m_timeoutTimes, after);
            int startLocationIndex = AssignArrayValue(m_startLocations, gameObject.transform.localPosition);
            int transitionTypeIndex = AssignArrayValue(m_transitions, transition);
            int elementTypeIndex = AssignArrayValue(m_elementTypes, elementType);
            int modesIndex = AssignArrayValue(m_modes, mode);

            return $"{objectIndex},,,{durationIndex},,,{startTimeIndex},,,{pixelOffsetIndex},,,{timeoutTimesIndex},,,{startLocationIndex},,,{transitionTypeIndex},,,{modesIndex},,,{elementTypeIndex}";
        }

        private const int OBJECT_INDEX = 0;
        private const int DURATION_INDEX = 1;
        private const int START_TIME_INDEX = 2;
        private const int PIXEL_OFFSET_INDEX = 3;
        private const int TIME_OUT_INDEX = 4;
        private const int START_LOCATION_INDEX = 5;
        private const int TRANSITION_TYPE_INDEX = 6;
        private const int ANIMATION_MODE_INDEX = 7;
        private const int ELEMENT_TYPE_INDEX = 8;

        private static int AssignArrayValue<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    array[i] = value;
                    return i;
                }

                continue;
            }

            return -1;
        }

        private static int AssignArrayValue(float[] array, float value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == -1)
                {
                    array[i] = value;
                    return i;
                }

                continue;
            }

            return -1;
        }

        private static int AssignArrayValue(int[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == -1)
                {
                    array[i] = value;
                    return i;
                }

                continue;
            }

            return -1;
        }

        private static int AssignArrayValue(Vector3[] array, Vector3 value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (IsUnassigned(array[i]))
                {
                    array[i] = value;
                    return i;
                }

                continue;
            }

            return -1;
        }

        private static int AssignArrayValue(TransitionType[] array, TransitionType value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == TransitionType.None)
                {
                    array[i] = value;
                    return i;
                }

                continue;
            }

            return -1;
        }

        private static int AssignArrayValue(AnimationMode[] array, AnimationMode value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == AnimationMode.None)
                {
                    array[i] = value;
                    return i;
                }

                continue;
            }

            return -1;
        }

        private static int AssignArrayValue(ElementType[] array, ElementType value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == ElementType.None)
                {
                    array[i] = value;
                    return i;
                }

                continue;
            }

            return -1;
        }

        private void Update()
        {
            int currentWorkingTasks = 0;
            for (int i = 0; i < m_currentTasks.Length; i++)
            {
                if (m_currentTasks[i] == null || m_currentTasks[i] == "") continue;
                currentWorkingTasks++;

                string[] parsedStrData = UdonUtils.ParseDataString(m_currentTasks[i]);

                Object targetObj = null;
                switch (m_elementTypes[int.Parse(parsedStrData[ELEMENT_TYPE_INDEX])])
                {
                    case ElementType.Text: targetObj = m_targetTexts[int.Parse(parsedStrData[OBJECT_INDEX])]; break;
                    case ElementType.TMP_Text: targetObj = m_targetTMPTexts[int.Parse(parsedStrData[OBJECT_INDEX])]; break;
                    case ElementType.Image: targetObj = m_targetImages[int.Parse(parsedStrData[OBJECT_INDEX])]; break;
                    case ElementType.RawImage: targetObj = m_targetRawImages[int.Parse(parsedStrData[OBJECT_INDEX])]; break;
                    case ElementType.Button: targetObj = m_targetButtons[int.Parse(parsedStrData[OBJECT_INDEX])]; break;
                }

                if (targetObj == null) continue;

                if ((Time.time - (m_startTimes[int.Parse(parsedStrData[START_TIME_INDEX])] + m_timeoutTimes[int.Parse(parsedStrData[TIME_OUT_INDEX])])) <= 0f) continue;

                float t = 1f;
                if (m_durations[int.Parse(parsedStrData[DURATION_INDEX])] > 0f)
                {
                    t = (Time.time - (m_startTimes[int.Parse(parsedStrData[START_TIME_INDEX])] + m_timeoutTimes[int.Parse(parsedStrData[TIME_OUT_INDEX])])) / m_durations[int.Parse(parsedStrData[DURATION_INDEX])];
                }

                if (t > 1f) t = 1f;

                float eased = ApplyEasing(t, m_transitions[int.Parse(parsedStrData[TRANSITION_TYPE_INDEX])]);

                switch (m_modes[int.Parse(parsedStrData[ANIMATION_MODE_INDEX])])
                {
                    case AnimationMode.FadeIn:
                        {
                            float alpha = eased;
                            switch (m_elementTypes[int.Parse(parsedStrData[ELEMENT_TYPE_INDEX])])
                            {
                                case ElementType.Text:
                                    Text text = (Text)targetObj;
                                    var c1 = text.color; c1.a = alpha; text.color = c1;
                                    break;
                                case ElementType.TMP_Text:
                                    TMP_Text tmp = (TMP_Text)targetObj;
                                    var c2 = tmp.color; c2.a = alpha; tmp.color = c2;
                                    break;
                                case ElementType.Image:
                                    Image img = (Image)targetObj;
                                    var c3 = img.color; c3.a = alpha; img.color = c3;
                                    break;
                                case ElementType.RawImage:
                                    RawImage raw = (RawImage)targetObj;
                                    var c4 = raw.color; c4.a = alpha; raw.color = c4;
                                    break;
                                case ElementType.Button:
                                    Button btn = (Button)targetObj;
                                    var c5 = btn.targetGraphic.color; c5.a = alpha; btn.targetGraphic.color = c5;
                                    break;
                            }
                            break;
                        }
                    case AnimationMode.FadeOut:
                        {
                            float alpha = 1f - eased;
                            switch (m_elementTypes[int.Parse(parsedStrData[ELEMENT_TYPE_INDEX])])
                            {
                                case ElementType.Text:
                                    Text text = (Text)targetObj;
                                    var c1 = text.color; c1.a = alpha; text.color = c1;
                                    break;
                                case ElementType.TMP_Text:
                                    TMP_Text tmp = (TMP_Text)targetObj;
                                    var c2 = tmp.color; c2.a = alpha; tmp.color = c2;
                                    break;
                                case ElementType.Image:
                                    Image img = (Image)targetObj;
                                    var c3 = img.color; c3.a = alpha; img.color = c3;
                                    break;
                                case ElementType.RawImage:
                                    RawImage raw = (RawImage)targetObj;
                                    var c4 = raw.color; c4.a = alpha; raw.color = c4;
                                    break;
                                case ElementType.Button:
                                    Button btn = (Button)targetObj;
                                    var c5 = btn.targetGraphic.color; c5.a = alpha; btn.targetGraphic.color = c5;
                                    break;
                            }
                            break;
                        }
                    case AnimationMode.MoveDown:
                        {
                            Vector3 newPos = m_startLocations[int.Parse(parsedStrData[START_LOCATION_INDEX])];
                            newPos.y += m_pixelOffsets[int.Parse(parsedStrData[PIXEL_OFFSET_INDEX])] * (1f - eased);
                            ((Component)targetObj).gameObject.transform.localPosition = newPos;
                            break;
                        }
                    case AnimationMode.MoveUp:
                        {
                            Vector3 newPos = m_startLocations[int.Parse(parsedStrData[START_LOCATION_INDEX])];
                            newPos.y -= m_pixelOffsets[int.Parse(parsedStrData[PIXEL_OFFSET_INDEX])] * (1f - eased);
                            ((Component)targetObj).gameObject.transform.localPosition = newPos;
                            break;
                        }
                    case AnimationMode.MoveLeft:
                        {
                            Vector3 newPos = m_startLocations[int.Parse(parsedStrData[START_LOCATION_INDEX])];
                            newPos.x += m_pixelOffsets[int.Parse(parsedStrData[PIXEL_OFFSET_INDEX])] * (1f - eased);
                            ((Component)targetObj).gameObject.transform.localPosition = newPos;
                            break;
                        }
                    case AnimationMode.MoveRight:
                        {
                            Vector3 newPos = m_startLocations[int.Parse(parsedStrData[START_LOCATION_INDEX])];
                            newPos.x -= m_pixelOffsets[int.Parse(parsedStrData[PIXEL_OFFSET_INDEX])] * (1f - eased);
                            ((Component)targetObj).gameObject.transform.localPosition = newPos;
                            break;
                        }
                }

                if (t >= 1f)
                {
                    Debug.Log($"[{UdonUtils.ColorizeString("Canvas Animation System", "#4eb3ee")}] Animation Finished!");
                    switch (m_elementTypes[int.Parse(parsedStrData[ELEMENT_TYPE_INDEX])])
                    {
                        case ElementType.Text: m_targetTexts[int.Parse(parsedStrData[OBJECT_INDEX])] = null; break;
                        case ElementType.TMP_Text: m_targetTMPTexts[int.Parse(parsedStrData[OBJECT_INDEX])] = null; break;
                        case ElementType.Image: m_targetImages[int.Parse(parsedStrData[OBJECT_INDEX])] = null; break;
                        case ElementType.RawImage: m_targetRawImages[int.Parse(parsedStrData[OBJECT_INDEX])] = null; break;
                        case ElementType.Button: m_targetButtons[int.Parse(parsedStrData[OBJECT_INDEX])] = null; break;
                    }

                    m_durations[int.Parse(parsedStrData[DURATION_INDEX])] = -1;
                    m_pixelOffsets[int.Parse(parsedStrData[PIXEL_OFFSET_INDEX])] = -1;
                    m_startTimes[int.Parse(parsedStrData[START_TIME_INDEX])] = -1;
                    m_timeoutTimes[int.Parse(parsedStrData[TIME_OUT_INDEX])] = -1;
                    m_startLocations[int.Parse(parsedStrData[START_LOCATION_INDEX])] = Vector3.positiveInfinity;
                    m_transitions[int.Parse(parsedStrData[TRANSITION_TYPE_INDEX])] = TransitionType.None;
                    m_modes[int.Parse(parsedStrData[ANIMATION_MODE_INDEX])] = AnimationMode.None;
                    m_elementTypes[int.Parse(parsedStrData[ELEMENT_TYPE_INDEX])] = ElementType.None;
                    m_currentTasks[i] = null;
                }
            }

            if (peakConcurrentAnimations < currentWorkingTasks) peakConcurrentAnimations = currentWorkingTasks;
            runningAnimations = currentWorkingTasks;
        }

        private float ApplyEasing(float t, TransitionType type)
        {
            switch (type)
            {
                case TransitionType.Linear:
                    return t;
                case TransitionType.EaseIn:
                    return t * t;
                case TransitionType.EaseOut:
                    return 1f - ((1f - t) * (1f - t));
                case TransitionType.EaseInOut:
                    return Mathf.SmoothStep(0f, 1f, t);
                default:
                    return t;
            }
        }

        private static bool IsUnassigned(Vector3 v)
        {
            return float.IsPositiveInfinity(v.x)
                && float.IsPositiveInfinity(v.y)
                && float.IsPositiveInfinity(v.z);
        }
    }
}
