namespace TemplateProject.xUnit.IntegrationTests.Endpoints;

internal  class EndpointConstants
{
    public static class UiApi
    {
        private const string BasePathV1 = "api/v1/";
        
        public static class Profile
        {
            private const string ControllerBasePathV1 = BasePathV1 + "profile/";
            
            public const string GetCurrent = ControllerBasePathV1 + "current";
        }
        
        public static class Auth
        {
            private const string ControllerBasePathV1 = BasePathV1 + "auth/";
            
            public const string SignIn = ControllerBasePathV1 + "sign-in";
            public const string Register = ControllerBasePathV1 + "register";
        }
    }
    
    public static class PublicApi
    {
        private const string BasePathV1 = "api/public/v1/";
    }
}