using System;
using System.Timers;
using Timer = System.Timers.Timer;

public class AdvancedTimer : IDisposable
{
    private TimeSpan _duration;
    private DateTime _startTime;
    private DateTime? _pauseTime;
    private TimeSpan _incrementAmount;
    private Timer _timer;
    private bool _isPasued;
    /* private int _timeBetweenEventCall; */
    /* private int _timeSinceLastEventCall; */

    // private Color _color;

    public event EventHandler Elapsed;
    public event EventHandler TimeRanOut;

    // , Color color
    public AdvancedTimer(TimeSpan duration, DateTime startTime)
    {
        _duration = duration;
        // _color = color;
        _incrementAmount = TimeSpan.FromSeconds(1);

        // sync client timers every 15 seconds
        _timer = new Timer(1000);
        _timer.Elapsed += TimerElapsed;

        _startTime = startTime;
        _pauseTime = DateTime.Now;
        _timer.Stop();

        _isPasued = true;
    }

    public TimeSpan RemainingTime
    {
        get
        {
            if (_pauseTime.HasValue)
                return _duration - (_pauseTime.Value - _startTime);
            else
                return _duration - (DateTime.Now - _startTime);
        }
    }

    public void Pause()
    {
        if (_isPasued) return;

        _pauseTime = DateTime.Now;
        _timer.Stop();
        _isPasued = true;
    }

    public void Resume()
    {
        if (!_pauseTime.HasValue || !_isPasued) return;


        var pausedDuration = _pauseTime.Value - _startTime;
        _startTime = DateTime.Now - pausedDuration;
        _pauseTime = null;
        _timer.Start();
        _isPasued = false;
    }

    public void Increment(int amount)
    {
        _duration = _duration.Add(TimeSpan.FromSeconds(amount));
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        //Console.WriteLine(DateTime.Now.TimeOfDay + ", " + this.RemainingTime.TotalSeconds);
        if (this.RemainingTime.TotalSeconds < 0)
        {
            EventHandler handler = TimeRanOut;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
