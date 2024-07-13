namespace WarringStates.Events;

public delegate void Callback();

public delegate void Callback<TArgs>(TArgs args);
