namespace QuickRoute.BusinessEntities.Actions
{
  /// <summary>
  /// Describes a user interface action that affects a session in some way. This is the base component for undo/redo handling. Actions are standalone objects that carry all 
  /// information inside themselves. However, to support undo/redo, they need to be propagated to the main application where they are stored in a stack.
  /// </summary>
  public interface IAction
  {
    void Execute();
    void Undo();
  }
}