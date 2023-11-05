import { AnimateObject } from "./AnimateObject";
import { AnimationScheduler } from "./AnimationScheduler";
import { Tween } from "./Tween";




/**
 * ȡ�� TWEEN ���䶯������
 */
export class AnimateTween extends AnimateObject {
    private _object: any = null;
    private _valuesStart: any = {};
    private _valuesEnd: any = {};
    private _valuesStartRepeat: any = {};
    private _duration = 1000;
    private _repeat = 0;
    private _yoyo = false;
    private _isPlaying = false;
    private _reversed = false;
    private _delayTime = 0;
    private _startTime: number = null;
    private _easingFunction: (k: number) => number = Tween.Easing.Linear.None;
    private _interpolationFunction: (v: number[], k: number) => number = Tween.Interpolation.Linear;
    private _chainedTweens: any = [];
    private _onStartCallback: (object?: any) => void = null;
    private _onStartCallbackFired = false;
    private _onUpdateCallback: (object?: any) => void = null;
    private _onCompleteCallback: (object?: any) => void = null;
    private _onStopCallback: (object?: any) => void = null;
    private _oneFinallys: ((object?: any) => void)[];

    public name: string;




    /**
     * ����һ��Tween��ʵ��
     * @param object ���ж������ԵĶ��� �� { x:0 , y:0 }
     */
    public constructor(scheduler: AnimationScheduler, object?: any) {
        super(scheduler);
        this._oneFinallys = [];
        if (object != null) {
            this.from(object);
        }
    }






    private schedulerFinally(complete: boolean) {
        if (this._oneFinallys.length > 0) {
            for (let i = 0; i < this._oneFinallys.length; i++) {
                this._oneFinallys[i].call(this);
            }
            this._oneFinallys.length = 0;
        }
    }




    /**
     * �ȴ��������\
     * �������δ���Ż򲥷���Ͻ�������\
     * ֹͣ���������stop����\
     * ���� TWEEN.removeʵ�ֵĶ���ֹͣ���ᵼ�¸÷�������
     */
    public async wait(): Promise<void> {
        if (!this._isPlaying) return;
        return new Promise<void>((resolve) => {
            this.onOneFinally(resolve);
        });
    }




    public get isPlaying(): boolean {
        return this._isPlaying;
    }



    /**
     * ͬ���캯��������ԭʼ����
     * @param object 
     */
    public from(object: any): this {
        this._object = object;
        return this;
    }


    /**
     * ���涯���ֶ�
     */
    private storeValues() {
        this._valuesStart = {};
        for (const field in this._object) {
            if (this._valuesEnd[field] != null && this._object[field] != null) {
                this._valuesStart[field] = parseFloat(this._object[field]);
            }
        }
    }



    /**
     * ����
     * @param properties ���������� �� { x:100 , y:100 }
     * @param duration  ��ɸö���������ʱ��
     */
    public to(properties: any, duration: number): this {
        if (duration !== undefined) {
            this._duration = duration;
        }
        this._valuesEnd = properties;
        this.storeValues();
        return this;
    };

    /**
     * ��ʼ����
     * @param time 
     */
    public start(): this {
        if (!this._isPlaying) {
            this.scheduler.add(this);
            this._isPlaying = true;
            this._onStartCallbackFired = false;
            this._startTime = this.scheduler.now();
            this._startTime += this._delayTime;
            for (const property in this._valuesEnd) {
                // Check if an Array was provided as property value
                if (this._valuesEnd[property] instanceof Array) {
                    if (this._valuesEnd[property].length === 0) {
                        continue;
                    }
                    // Create a local copy of the Array with the start value at the front
                    this._valuesEnd[property] = [this._object[property]].concat(this._valuesEnd[property]);
                }
                // If `to()` specifies a property that doesn't exist in the source object,
                // we should not set that property in the object
                if (this._valuesStart[property] === undefined) {
                    continue;
                }
                this._valuesStart[property] = this._object[property];
                if ((this._valuesStart[property] instanceof Array) === false) {
                    this._valuesStart[property] *= 1.0; // Ensures we're using numbers, not strings
                }
                this._valuesStartRepeat[property] = this._valuesStart[property] || 0;
            }
        }
        return this;

    };

    /**
     * ֹͣ����
     */
    public stop(): this {
        if (!this._isPlaying) {
            return this;
        }
        this.scheduler.remove(this);
        this._isPlaying = false;
        if (this._onStopCallback !== null) {
            this._onStopCallback.call(this._object);
        }
        this.schedulerFinally(false);
        this.stopChainedTweens();
        return this;

    };

    public stopChainedTweens() {
        for (let i = 0, numChainedTweens = this._chainedTweens.length; i < numChainedTweens; i++) {
            this._chainedTweens[i].stop();
        }
    };

    /**
     * �ӳ�
     * @param amount 
     */
    public delay(amount: number): this {
        this._delayTime = amount;
        return this;
    };

    /**
     * �ظ�ִ��
     * @param times  �ظ�����
     */
    public repeat(count: number): this {
        this._repeat = count;
        return this;
    };

    /**
     * yoyo
     * @param yoyo 
     */
    public yoyo(yoyo: boolean): this {
        this._yoyo = yoyo;
        return this;
    };

    /**
     * ��������
     * @param easing ����
     */
    public easing(easing: (k: number) => number): this {
        this._easingFunction = easing;
        return this;
    };

    /**
     * ��ֵ����
     * @param interpolation 
     */
    public interpolation(interpolation: (v: number[], k: number) => number): this {
        this._interpolationFunction = interpolation;
        return this;
    };

    /**
     * �岹 ����
     * @param param 
     */
    public chain(...param: AnimateTween[]): this {
        this._chainedTweens = param;//arguments
        return this;
    };

    /**
     * ������ʼ�ص�
     * @param callback 
     */
    public onStart(callback: (object?: any) => void): this {
        this._onStartCallback = callback;
        return this;
    };

    /**
     * ÿ�θ��»ص�
     * @param callback 
     */
    public onUpdate(callback: (object?: any) => void): this {
        this._onUpdateCallback = callback;
        return this;
    };

    /**
     * ��ɻص�
     * @param callback 
     */
    public onComplete(callback: (object?: any) => void): this {
        this._onCompleteCallback = callback;
        return this;
    };

    /**
     * ֹͣ�ص�
     * @param callback 
     */
    public onStop(callback: (object?: any) => void): this {
        this._onStopCallback = callback;
        return this;
    };

    /**
     * һ�����ջص�\
     * ����ɻ�ֹͣʱ����
     */
    public onOneFinally(callback: () => void): this {
        this._oneFinallys.push(callback);
        return this;
    }



    public update(tick: number, elapsedTime: number): boolean {
        let property;
        let elapsed;
        const time = this.scheduler.now();
        if (time < this._startTime) {
            return true;
        }
        if (this._onStartCallbackFired === false) {
            if (this._onStartCallback !== null) {
                this._onStartCallback.call(this._object);
            }
            this._onStartCallbackFired = true;
        }
        elapsed = (time - this._startTime) / this._duration;
        elapsed = elapsed > 1 ? 1 : elapsed;
        const value = this._easingFunction(elapsed);
        for (property in this._valuesEnd) {

            // Don't update properties that do not exist in the source object
            if (this._valuesStart[property] === undefined) {
                continue;
            }

            const start = this._valuesStart[property] || 0;
            let end = this._valuesEnd[property];

            if (end instanceof Array) {

                this._object[property] = this._interpolationFunction(end, value);

            } else {

                // Parses relative end values with start as base (e.g.: +10, -3)
                if (typeof (end) === 'string') {

                    if (end.charAt(0) === '+' || end.charAt(0) === '-') {
                        end = start + parseFloat(end);
                    } else {
                        end = parseFloat(end);
                    }
                }

                // Protect against non numeric properties.
                if (typeof (end) === 'number') {
                    this._object[property] = start + (end - start) * value;
                }

            }

        }
        if (this._onUpdateCallback !== null) {
            this._onUpdateCallback.call(this._object, value);
        }
        if (elapsed === 1) {

            if (this._repeat > 0) {

                if (isFinite(this._repeat)) {
                    this._repeat--;
                }

                // Reassign starting values, restart by making startTime = now
                for (property in this._valuesStartRepeat) {

                    if (typeof (this._valuesEnd[property]) === 'string') {
                        this._valuesStartRepeat[property] = this._valuesStartRepeat[property] + parseFloat(this._valuesEnd[property]);
                    }

                    if (this._yoyo) {
                        const tmp = this._valuesStartRepeat[property];

                        this._valuesStartRepeat[property] = this._valuesEnd[property];
                        this._valuesEnd[property] = tmp;
                    }

                    this._valuesStart[property] = this._valuesStartRepeat[property];

                }

                if (this._yoyo) {
                    this._reversed = !this._reversed;
                }

                this._startTime = time + this._delayTime;

                return true;

            } else {
                if (this._onCompleteCallback !== null) {
                    this._onCompleteCallback.call(this._object);
                }
                for (let i = 0, numChainedTweens = this._chainedTweens.length; i < numChainedTweens; i++) {
                    // Make the chained tweens start exactly at the time they should,
                    // even if the `update()` method was called way past the duration of the tween
                    this._chainedTweens[i].start(this._startTime + this._duration);
                }
                this._isPlaying = false;
                this.schedulerFinally(true);
                return false;

            }

        }

        return true;

    };

}
