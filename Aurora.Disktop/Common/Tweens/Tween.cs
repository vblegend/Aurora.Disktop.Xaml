using Microsoft.Xna.Framework;

using System.ComponentModel;


namespace Aurora.Disktop.Common.Tweens
{

    public interface ITweenUpdateable
    {
        Boolean Update(GameTime time);
    }



    public partial class Tween
    {
        public Boolean IsCompleted { get; protected set; }
        public TimeSpan Duration = new TimeSpan(0, 0, 1);
        public EasingFunction Easing = Tweens.Easing.Linear.None;
    }


    public abstract class Tween<DataType> : Tween, ITweenUpdateable
    {
        public DataType from;
        public DataType to;
        private TimeSpan _startTime;


        public void ChangeTo(DataType to, TimeSpan duration)
        {
            if (duration.Ticks == 0) this.from = to;
            this.to = to;
            this.Duration = duration;
            this.IsCompleted = duration.Ticks == 0;
            return;
        }


        public Tween<DataType> ChangeTo(DataType from, DataType to, TimeSpan duration)
        {
            if (_startTime.TotalMilliseconds == 0)
            {
                this.from = from;
                this.to = to;
                this.Duration = duration;
                this.IsCompleted = false;
            }
            return this;
        }

        bool ITweenUpdateable.Update(GameTime time)
        {
            if (this.IsCompleted) return false;
            if (this.Duration.Ticks == 0) return false;

            var elapsed = (time.TotalGameTime - this._startTime) / this.Duration;
            elapsed = elapsed > 1 ? 1 : elapsed;
            var value = this.Easing(elapsed);
            var dest = this.Lerp(this.from, this.to, value);
            this.Apply(dest);
            if (elapsed == 1)
            {
                this._startTime = new TimeSpan(0, 0, 0);
                this.IsCompleted = true;
                return false;
            }
            return true;
        }

        public DataType Value
        {
            get
            {
                return this.from;
            }
        }





        internal abstract DataType Lerp(DataType from, DataType to, Double time);
        internal abstract void Apply(DataType value);
    }







}
