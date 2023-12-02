using Microsoft.Xna.Framework;


namespace Aurora.UI.Animation
{
    public delegate void TickEventHandler<T>(T sender) where T : Ticker;

    public partial class Ticker : IAnimationUpdateable
    {
        public bool IsCompleted { get; protected set; }
        /// <summary>
        /// 执行间隔
        /// </summary>
        public TimeSpan Interval = new TimeSpan(0, 0, 1);
        /// <summary>
        /// 延迟启动
        /// </summary>
        public TimeSpan Delay = new TimeSpan(0, 0, 0);

        /// <summary>
        /// 持续总时长
        /// </summary>
        public TimeSpan Duration = new TimeSpan(0, 0, 0);

        /// <summary>
        /// 持续总时长
        /// </summary>
        public Int32 Count = -1;

        private TimeSpan intervalTick;
        private TimeSpan? _startTime;

        private Boolean isRuning;

        public event TickEventHandler<Ticker> Tick;



        bool IAnimationUpdateable.Update(GameTime time)
        {
            if (!this.isRuning) return false;
            if (this._startTime == null)
            {
                this._startTime = time.TotalGameTime;
                return true;
            }
            if (time.TotalGameTime - Delay < this._startTime)
            {
                return false;
            }

            if (this.Duration.Ticks > 0)
            {
                if (time.TotalGameTime - this._startTime.Value > this.Duration)
                {
                    this.isRuning = false;
                    this._startTime = null;
                    return false;
                }
            }
            intervalTick += time.ElapsedGameTime;
            if (intervalTick < this.Interval) return true;
            this.Tick?.Invoke(this);
            return true;
        }




        public void Start()
        {
            this.intervalTick = new TimeSpan();
            this._startTime = null;
            this.isRuning = true;
        }

        public void Stop()
        {
            this.isRuning = false;
        }




    }












}
