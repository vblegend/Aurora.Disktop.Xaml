using Aurora.UI.Controls;
using Microsoft.Xna.Framework;


namespace Aurora.UI.Tweens
{

    public interface ITweenUpdateable
    {
        bool Update(GameTime time);
    }


    public delegate void TweenCompleteEventHandler<T>(T sender) where T : Tween;
    public partial class Tween
    {
        public bool IsCompleted { get; protected set; }
        public TimeSpan Duration = new TimeSpan(0, 0, 1);
        public EasingFunction Easing = Tweens.Easing.Linear.None;
    }


    public abstract class Tween<TweenType, DataType> : Tween, ITweenUpdateable where TweenType : Tween
    {
        private DataType from;
        private DataType to;
        private DataType value;
        private TimeSpan? _startTime;





        /// <summary>
        /// 立即改变值
        /// </summary>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        public void ChangeTo(DataType to)
        {
            this.value = to;
            this.from = value;
            this.to = to;
            Duration = new TimeSpan(0);
            IsCompleted = true;
            _startTime = null;
            return;
        }




        /// <summary>
        /// 改变值
        /// </summary>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        public void ChangeTo(DataType to, TimeSpan duration)
        {
            if (duration.Ticks == 0)
            {
                value = to;
            }
            from = value;
            this.to = to;
            Duration = duration;
            IsCompleted = duration.Ticks == 0;
            _startTime = null;
            return;
        }

        /// <summary>
        /// 改变当前值和目标值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Tween<TweenType, DataType> ChangeTo(DataType from, DataType to, TimeSpan duration)
        {
            if (!_startTime.HasValue)
            {
                this.from = from;
                this.to = to;
                Duration = duration;
                IsCompleted = false;
            }
            return this;
        }

        bool ITweenUpdateable.Update(GameTime time)
        {
            if (IsCompleted) return false;
            if (Duration.Ticks == 0) return false;
            if (!_startTime.HasValue)
            {
                _startTime = time.TotalGameTime;
                this.OnBegin?.Invoke(this as TweenType);
            }
            var elapsed = (time.TotalGameTime - _startTime.Value) / Duration;
            elapsed = elapsed > 1 ? 1 : elapsed;
            var value = Easing(elapsed);
            this.value = Lerp(from, to, value);

            if (elapsed == 1)
            {
                _startTime = null;
                IsCompleted = true;
                this.OnComplete?.Invoke(this as TweenType);
                return false;
            }
            return true;
        }

        public DataType Value
        {
            get
            {
                return value;
            }
        }


        public DataType ToValue
        {
            get
            {
                return to;
            }
        }


        public event TweenCompleteEventHandler<TweenType> OnBegin;

        public event TweenCompleteEventHandler<TweenType> OnComplete;


        internal abstract DataType Lerp(DataType from, DataType to, double time);
    }







}
