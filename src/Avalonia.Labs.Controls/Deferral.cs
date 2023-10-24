using System;

namespace Avalonia.Labs;

/// <summary>
/// Stores a <see cref="Action"/> to be invoked upon completion of the 
/// deferral and manipulates the state of the deferral.
/// </summary>
public class Deferral
{
    /// <summary>
    /// Initializes a new Deferral object and specifies a <see cref="Action"/> to be called 
    /// upon completion of the deferral. 
    /// </summary>
    /// <param name="completedHandler">A <see cref="Action"/> to be called upon completion of the deferral.</param>
    public Deferral(Action completedHandler)
    {
        if (completedHandler is null)
            throw new ArgumentNullException(nameof(completedHandler), "Completion delegate cannot be null");

        _handler = completedHandler;
    }

    /// <summary>
    /// If the DeferralCompletedHandler has not yet been invoked, this will call it and drop the reference to the delegate.
    /// </summary>
    public void Complete()
    {
        _handler.Invoke();
    }

    private readonly Action _handler;
}
