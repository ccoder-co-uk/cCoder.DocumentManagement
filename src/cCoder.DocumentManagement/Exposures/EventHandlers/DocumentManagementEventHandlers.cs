using cCoder.DocumentManagement.Services.Foundations.Events;


namespace cCoder.DocumentManagement.Exposures.EventHandlers;

internal class DocumentManagementEventHandlers(IEventHandlerService eventHandlerService)
    : IDocumentManagementEventHandlers
{
    public void ListenToAllEvents() => eventHandlerService.ListenToAllEvents();
}


