namespace FileUploadSystem.Web.Infrastructure.ServiceHelpers
{
    public class ServiceReview
    {
        public ServiceReview()
        {
        }

        public ServiceReview (string successMessage, int? redirectId, bool error)
        {
            this.Message = successMessage;
            this.RedirectId = redirectId;
            this.ErrorFound = error;
        }

        public bool ErrorFound { get; set; }

        public string Message { get; set; }

        public int? RedirectId { get; set; }
    }
}