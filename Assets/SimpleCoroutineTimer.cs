using UnityEngine;
using System.Collections;
using System;

namespace QXToolkits.Utility
{
    public class SimpleCoroutineTimer
    {
        public bool Running //运行
        {
            get { return running; }
        }

        public bool Paused //暂停
        {
            get { return paused; }
        }

        public float Interval//时间间隔
        {
            get { return interval; }
            set { interval = value; }
        }

        public delegate void TickHandler(object sender,EventArgs e);

        public event TickHandler Tick;

        bool running;//云顶返回值变量
        bool paused; //暂停返回值变量
        bool stopped;//停止的局部变量

        //构造函数
        public SimpleCoroutineTimer(MonoBehaviour autoDestroyOwner)
        {
            this._autoDestroyOwner = autoDestroyOwner;
            this._hasAutoDestroyOwner = autoDestroyOwner != null;
        }

        float elapsedTime = 0.0f;//已消耗的时间
        float interval = 0.5f;
        private bool ignoreTimeScale = false;

        private bool isFirstRun = false;
        private MonoBehaviour _autoDestroyOwner;//构造函数的变量,自动销毁自身
        private bool _hasAutoDestroyOwner;
        private bool isOwnerDestroyed
        {
            get { return this._hasAutoDestroyOwner && this._autoDestroyOwner == null; }
        }     

        /// <summary>
        /// Get whether or not the timer has finished running for any reason.
        /// </summary>
        public bool isDone
        {
            get { return isOwnerDestroyed; }
        }

        public void Pause()
        {
            paused = true;
        }

        public void Unpause()
        {
            paused = false;
        }

        public void Start()
        {
            running = true;
            _autoDestroyOwner.StartCoroutine(CallWrapper());
        }

        public void Stop()
        {
            stopped = true;
            running = false;
            //stopAllCoroutines();
        }

        private IEnumerator TimerTickEnumerator()
        {
            if (Tick==null)
            {
                yield break;
            }

            if (isFirstRun)
            {
                isFirstRun = false;
                Tick(this,null);
            }

            while (running)
            {
                if (elapsedTime >= Interval)
                {
                    elapsedTime = elapsedTime - Interval;
                    Tick(this,null);
                }
                else
                {
                    elapsedTime += (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
                }
                yield return null;
            }
        }
        /// <summary>
        /// 外层Enumerator 负责控制
        /// </summary>
        /// <returns></returns>
         IEnumerator CallWrapper()
        {
            yield return null;
            IEnumerator e = TimerTickEnumerator();
            while (running)
            {
                if (paused)
                yield return null;
                else{
                    if(this.isDone)
                    {
                        running = false;
                    }
                    if (e!=null && e.MoveNext())
                    {
                        yield return e.Current;
                    }
                    else
                    {
                        running = false;
                    }
                }
            }
        }
    }
}

