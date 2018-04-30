using System.Web.Http;
using WebActivatorEx;
using Financial.Api;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Financial.Api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "FinancialApi");
                        c.PrettyPrint();

                        c.BasicAuth("basic").Description("Basic HTTP Authentication");
                        //
                        // NOTE: You must also configure 'EnableApiKeySupport' below in the SwaggerUI section
                        //c.ApiKey("apiKey")
                        //    .Description("API Key Authentication")
                        //    .Name("apiKey")
                        //    .In("header");
                        //
                        //c.OAuth2("oauth2")
                        //    .Description("OAuth2 Implicit Grant")
                        //    .Flow("implicit")
                        //    .AuthorizationUrl("http://petstore.swagger.wordnik.com/api/oauth/dialog")
                        //    //.TokenUrl("https://tempuri.org/token")
                        //    .Scopes(scopes =>
                        //    {
                        //        scopes.Add("read", "Read access to protected resources");
                        //        scopes.Add("write", "Write access to protected resources");
                        //    });

                        // Swashbuckle makes a best attempt at generating Swagger compliant JSON schemas for the various types
                        // exposed in your API. However, there may be occasions when more control of the output is needed.
                        // This is supported through the "MapType" and "SchemaFilter" options:
                        //
                        // Use the "MapType" option to override the Schema generation for a specific type.
                        // It should be noted that the resulting Schema will be placed "inline" for any applicable Operations.
                        // While Swagger 2.0 supports inline definitions for "all" Schema types, the swagger-ui tool does not.
                        // It expects "complex" Schemas to be defined separately and referenced. For this reason, you should only
                        // use the "MapType" option when the resulting Schema is a primitive or array type. If you need to alter a
                        // complex Schema, use a Schema filter.
                        //
                        //c.MapType<ProductType>(() => new Schema { type = "integer", format = "int32" });

                        // If you want to post-modify "complex" Schemas once they've been generated, across the board or for a
                        // specific type, you can wire up one or more Schema filters.
                        //
                        //c.SchemaFilter<ApplySchemaVendorExtensions>();

                        // In a Swagger 2.0 document, complex types are typically declared globally and referenced by unique
                        // Schema Id. By default, Swashbuckle does NOT use the full type name in Schema Ids. In most cases, this
                        // works well because it prevents the "implementation detail" of type namespaces from leaking into your
                        // Swagger docs and UI. However, if you have multiple types in your API with the same class name, you'll
                        // need to opt out of this behavior to avoid Schema Id conflicts.
                        //
                        //c.UseFullTypeNameInSchemaIds();

                        // Alternatively, you can provide your own custom strategy for inferring SchemaId's for
                        // describing "complex" types in your API.
                        //
                        //c.SchemaId(t => t.FullName.Contains('`') ? t.FullName.Substring(0, t.FullName.IndexOf('`')) : t.FullName);

                        c.DescribeAllEnumsAsStrings();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("Financial API Swagger UI");

                        // If your API supports the OAuth2 Implicit flow, and you've described it correctly, according to
                        // the Swagger 2.0 specification, you can enable UI support as shown below.
                        //
                        //c.EnableOAuth2Support(
                        //    clientId: "test-client-id",
                        //    clientSecret: null,
                        //    realm: "test-realm",
                        //    appName: "Swagger UI"
                        //    //additionalQueryStringParams: new Dictionary<string, string>() { { "foo", "bar" } }
                        //);

                        // If your API supports ApiKey, you can override the default values.
                        // "apiKeyIn" can either be "query" or "header"
                        //
                        //c.EnableApiKeySupport("apiKey", "header");
                    });
        }
    }
}
