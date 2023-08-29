namespace CommandsServices.EventProcessing
{
    public interface IEventProcessor  
    {
        void ProcessEvent(string message);
    }  
}