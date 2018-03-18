
namespace Hado.Utils.ObjectPool
{
    /// <summary>
    /// see https://docs.unity3d.com/2017.3/Documentation/Manual/ExecutionOrder.html
    /// </summary>
    public class EventFunctionsReceiver : PoolManagedBehaviour
    {
        public bool IsAwakeCalled { get; private set; }

        public int OnEnableCount { get; private set; }

        public bool IsStartCalled { get; private set; }

        public int OnDisableCount { get; private set; }

        /// <summary>
        /// This function is always called before any Start functions and also just after a prefab is instantiated.
        /// (If a GameObject is inactive during start up Awake is not called until it is made active.)
        /// </summary>
        protected override void Awake()
        {
            IsAwakeCalled = true;
        }

        /// <summary>
        /// (only called if the Object is active)
        /// This function is called just after the object is enabled.
        /// This happens when a MonoBehaviour instance is created, such as when a level is loaded or a GameObject with the script component is instantiated.
        /// </summary>
        void OnEnable()
        {
            OnEnableCount += 1;
        }

        /// <summary>
        /// Start is called before the first frame update only if the script instance is enabled.
        /// </summary>
        void Start()
        {
            IsStartCalled = true;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            OnDisableCount += 1;
        }
    }
}