namespace FileUploadSystem.Web.Infrastructure.Services.Base
{
    using FileUploadSystem.Data;
    using FileUploadSystem.Data.Contracts;

    public abstract class BaseServices
    {
        protected IFileUploadSystemData Data { get; private set; }

        public BaseServices(IFileUploadSystemData data)
        {
            this.Data = data;
        }
    }
}