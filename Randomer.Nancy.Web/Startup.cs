using Owin;

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseNancy();
    }
}

public class IndexModule : Nancy.NancyModule
{
    public IndexModule()
    {
        Get["/"] = parameters =>
        {
            return View["index"];
        };
    }
}