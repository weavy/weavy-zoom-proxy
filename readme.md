# Weavy Zoom Proxy

Source code for Weavy Zoom Proxy - a proxy managing Zoom Webhook notifications to Weavy tenants.

## Introduction
Weavy allows developers that integrate any of the Weavy Messenger, Feeds or Comments app into their product to enable the Zoom integration. Once the Zoom integration is setup (https://docs.weavy.com/server/integrations/zoom), a meeting can be created directly inside Weavy.
So, what about this proxy? Well, even though Zoom allows a Zoom Marketplace app to work on any subdomain, the webhook notifications don't. Weavy use the Zoom webhooks to inform the end users when a meeting has ended and when (if) a cloud recording of that meeting is available. When that happens, the Zoom cards in Weavy are updated.
This is why we built this Weavy Zoom Proxy, to handle Zoom notifications on any subdomain.

### How does it work?
Zoom requires you to set an url to where Zoom should push the webhook notifications. This url should point to this Weavy Zoom Proxy. The proxy stores all the incoming notifications in a database. The database contains information about the meeting and the incoming event. At this moment, we support **End Meeting** and **All recordings have completed**.
The Weavy instance/tenant then have a Weavy Daemon (https://docs.weavy.com/server/development/daemons) that periodically polls the proxy for changes. If a change to a meeting is found, the events are returned from the proxy to the Weavy instance and the meeting is updated. When the meeting in Weavy is updated, the corresponding Zoom card in the Messenger, Feeds or Comments app are updated in real-time.

## How to setup the proxy

Setting up the proxy web site is quite easy. If you haven't already created a Zoom Marketplace app, that could involve some configuration and tweaking before it's approved by the Zoom Team though. We have tried to describe each needed step (3) for creating an Zoom app below to make the submission to the Zoom Team as hassle free as possible.
Just follow the steps below. Each step is described in more detail further down.

1. Clone the repo.
2. Create a new database in SQL Server.
3. Create a new Zoom Marketplace app (or update your existing one).
4. Update appsettings.json with the Zoom app credentials.
5. Add content to the proxy site that describes the usage of the integration to Weavy/Your webapp.
6. Publish the site.

### 1. Clone the repo
Clone this repo to your computer. Open up the solution in Visual Studio.

### 2. Create a new database
Create a new database in SQL Server. Update the appsettings.json file with the correct connection string. Make sure you have set the correct permissions to enable the web site to read/write to the database.

Run the `schema.sql` script, located in the `\Data` folder, on the database to create the necessary table.

### 3. Create a new Zoom Marketplace app
If you havn't already created a Zoom app on the Zoom Marketplace, go to https://marketplace.zoom.us/ and sign in with your account. Follow the steps below to create a new Zoom app required for the Weavy Zoom integration to work.

1. Select Develop ->  Build App.
2. Select **OAuth**  app type.
3. Give the app a name, for example **"[YOUR PRODUCTNAME] Meetings"**
4. Select **User-managed app**
5. Select that you want to publish the app on the Zoom App Marketplace
6. *App Credentials*:
	- **Redirect URL for OAuth**: This should be in the format https://any.yourdomain.com. The **any** keywork allows you to use the Zoom app on any subdomain for the **yourdomain.com**. You should of course replace **yourdomain.com** with the domain you use to host your Weavy tenants.
	- **Whitelist URL**: This should be https://yourdomain.com. Again, replace **yourdomain.com** with the domain you use to host your Weavy tenants
7. *Information*
	- Here you should describe the Zoom app. Add relevant images and screenshots from your web app that shows the usage of Weavy and the Zoom integration in the Messenger or Feeds/Comments app.
	- **Links**: The url's for Privacy policy, Terms of use, Support and Documentation **should** point to the corresponding pages on the Weavy Zoom Proxy site that you cloned above. If you for example publish the proxy site to https://zoom.yourdomain.com, this is the base url for the pages as well.
	- **Direct landing URL**: Select **From you landing page** here. The index page on the Proxy site contains information about the app and how to install it. Step **5** in the setup process for the proxy describes in more detail what the content pages should lool like.
	- **Deauthorization Notification URL**: This should point to the api endpoint `/api/events` on the proxy site. If you for example publish the proxy site to https://zoom.yourdomain.com, the url should be `https://zoom.yourdomain.com/api/events`.
8. *Feature*
	- Make sure **Event subscriptions** are enabled. Create a new Event subscription. Set the **Event notification endpoint URL [production]** to the proxy site end point `/api/events`. If you for example publish the proxy site to https://zoom.yourdomain.com, the url should be `https://zoom.yourdomain.com/api/events`. Add the *Event types* `Meeting -> End meeting` and `Recording -> All recording have completed`. Save!
	- Take note of the *Verification token*. You need it in the next step (4).
9. *Scopes*
	- The following scopes must be added: **View your meetings**, **View and manage your meetings**, **View your recordings** and **View your user profile**. Describe why and how the scopes are used. You can take a look at the existing **Weavy Meetings** app in the Zoom Marketplace for details how each scope are used.
10. *Submit*
	- Now it's time to submit the app to the Zoom Team for review. Follow the steps on the page to validate the domain. This is where you published to proxy site.  If you for example publish the proxy site to https://zoom.yourdomain.com, that is the url you should validate.
	- For the Zoom Team to be able to test the app in your environment, setup a test site of your product with any of the Weavy Messenger, Feed or Comments app. Make site the Weavy instance is configured with the Zoom app credentials. For more information on how to configure Weavy with the correct settings, go to docs.weavy.com/server/integrations/zoom#configuring-weavy. 
	- The Zoom Team will review your app and get back to you with additional information and requirements before the app will be published.


### 4. Update appsettings.json with the Zoom app credentials
- Take the Zoom Notification token from the Zoom app you just created and add it to appsettings.json. You should also add the same token to any Weavy tenant/instance. The token is the shared secret between any Weavy instance and the proxy.  For more information on how to configure Weavy with the correct verification token, go to docs.weavy.com/server/integrations/zoom#configuring-weavy. 
- Add the Zoom app's client id and secret to appsettings.json.

### 5. Add content
Zoom Marketplace requires that the following content is available for each app published on their store:
- Privacy Policy
- Terms of Use
- Support
- Documentation

For your convenience, we have prepared some example content for these pages. This is the same content that we use on the **Weavy Meetings** app already on the Zoom Marketplace. Feel free to reuse these pages and change the Company and Product names to match your requirements. We recommend that you create your own pages though!

> Please notice that these pages are just examples and everything may not be relevant or according to your terms of use or policys!

### 6. Publish
Publish the Weavy Zoom Proxy site to make it available for end users and of course the Zoom Team when reviewing your app.
Test the proxy by creating a new Zoom meeting in Weavy. When the meeting has ended and/or a cloud recording has completed processing, the meeting should be updated accordingly. 
