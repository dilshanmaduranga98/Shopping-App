namespace ShoppingCart.Infrastructure.AuthServicec
{

    // Class for storing authentication configurations
    public class KeyConfigurations
    {
        // Auth0 Domain
        public string Domain { get; set; }

        // Auth0 Audience
        public string Audience { get; set; }


        // Auth0 ClientId
        public string ClientId { get; set; }


        // Auth0 ClientSecret
        public string ClientSecret { get; set; }


        //Stripe secret key
        public string StripeKey { get; set; }


        //Send Grid secret key
        public string SendGridKey { get; set; }

    }
}
