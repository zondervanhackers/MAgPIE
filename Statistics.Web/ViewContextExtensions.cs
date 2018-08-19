namespace System.Web.Mvc
{
    public static class ViewContextExtensions
    {
        public static String IsActiveController(this ViewContext context, String controller)
        {
            return ((String)context.RouteData.Values["Controller"] == controller) ? "active" : "inactive";
        }
    }
}