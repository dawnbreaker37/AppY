using AppY.ViewModels;

namespace AppY.Abstractions
{
    public abstract class ChatMessageAbstraction : Message
    {
        public abstract Task<string?> Forward(ForwardMessage Model);
    }
}
