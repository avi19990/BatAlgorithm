interface IBatState
{
    public void Start(BatManager manager);
    public bool Update();

    public BatStateType BatStateType { get; }
}
