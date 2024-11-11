namespace Talabat.Apis.Extentions
{
    public static class AddSwaggerExtention
    {
      public static WebApplication UseSwaggerMiddlewares( this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
