using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BookStore.API.Conventions
{
    public class RemoveControllersConvention : IApplicationModelConvention
    {
        private readonly Type[] _controllersToRemove;

        public RemoveControllersConvention(params Type[] controllersToRemove)
        {
            _controllersToRemove = controllersToRemove;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers.ToList())
            {
                if (_controllersToRemove.Contains(controller.ControllerType))
                {
                    application.Controllers.Remove(controller);
                }
            }
        }
    }
}
