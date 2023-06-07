namespace TemplateProject.IntegrationTests.Testing;

public static class Endpoints
{
    public static class V1
    {
        private const string VersionPrefix = "v1";

        public static class Auth
        {
            private const string Controller = "auth";

            public static string SignUp() => VersionPrefix + $"/{Controller}" + "/register";
            public static string SignIn() => VersionPrefix + $"/{Controller}" + "/sign-in";
            public static string Refresh() => VersionPrefix + $"/{Controller}" + "/refresh";
        }
    }
}