namespace Loxone.Client.Commands
{
    using System.Threading.Tasks;

    public interface ICommandInvoker
    {
        CommandBase Command { get; set; }

        Task ExecuteAsync();
    }
}