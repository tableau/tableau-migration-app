# Tableau Migration App
### Table of Contents

- [Tableau Migration App](#tableau-migration-app)
		- [Table of Contents](#table-of-contents)
	- [Features](#features)
	- [Usage](#usage)
		- [Tableau Server and Cloud URLs](#tableau-server-and-cloud-urls)
		- [Personal Access Tokens](#personal-access-tokens)
		- [User Mappings](#user-mappings)
			- [Username already in email format](#username-already-in-email-format)
			- [User has an associated email](#user-has-an-associated-email)
			- [Default Domain Mapping](#default-domain-mapping)
			- [CSV Mapping](#csv-mapping)
		- [Outputs](#outputs)
		- [Logging](#logging)
		- [Canceling a Migration](#canceling-a-migration)
		- [Resuming a Migration](#resuming-a-migration)
		- [FAQ](#faq)
			- [The application is taking a long time, is it stuck?!](#the-application-is-taking-a-long-time-is-it-stuck)
			- [How do I migrate items with more specific logic rules?](#how-do-i-migrate-items-with-more-specific-logic-rules)
			- [How do I exclude certain items from being migrated?](#how-do-i-exclude-certain-items-from-being-migrated)
			- [What order will my resources get migrated in?](#what-order-will-my-resources-get-migrated-in)

## Features
**You can download the Tableau Migration App from the [releases](https://github.com/tableau/tableau-migration-app/releases) page of this repo.**

The purpose of the Tableau Migration App is to provide users with a method to perform simple migrations from their Tableau Server to Tableau Cloud. This app is meant to accompany the [Tableau Manual Migration Guide](https://help.tableau.com/current/guides/migration/en-us/emg_intro.htm) and replace the work required for the migration steps.

<div align="center">
	<img src="/screenshots/TableauMigrationApp.png" alt="Tableau Migration App" width=400>
</div>

* The migration app uses the [Tableau Migration SDK](https://github.com/tableau/tableau-migration-sdk) under the hood, and supports all migration resources that the Migration SDK offers.
  * The Migration SDK currently includes:
	*  Users
	*  Groups
	*  Projects
	*  Data Sources
	*  Workbooks
	*  Extract Refresh Tasks
	*  Custom Views

* Basic mapping options are provided in the app for username migrations.
* A simplified view of ongoing migration progress.

## Usage
### Tableau Server and Cloud URLs
> <div align="center"><img src="/screenshots/UrlParams.png" alt="Tableau Migration App URL Fields" ></div>
The fields in this section are for providing the URIs of the Tableau Server you wish to migrate from, and the Tableau Cloud you wish to migrate to.

These URIs can be found in your browser address bar when logging into the respective Tableau products, or are what you use when connecting from Tableau Desktop.

> The `Base URI` and `Site Name` fields will automatically extract the appropriate information based on the URI provided. Double check that these are correct before continuing.

**Note**: Multi-site environments show the `/site/` as part of the URI. If yours doesn’t have one, it isn’t a problem, and just means that you only have a single site; the `Default` site.

### Personal Access Tokens
> <div align="center"><img src="/screenshots/PatParams.png" alt="Tableau Migration App Token Fields" ></div>

The app uses personal access tokens (PATs) to programmatically access and make changes to the corresponding Tableau products.

To generate PATs in your Tableau Environment:

1. Go to **Users > {Your User Here} > Settings > Personal Access Tokens**.
2. Provide a **Token name**.
3. Click the **Create Token** button.
   > <div align="center"><img src="/screenshots/CreatePAT.png" alt="Tableau Migration PAT" ></div>
4. After it’s created, a `secret` will be displayed to you for that PAT. Click **Copy Secret**.

   > <div align="center"><img src="/screenshots/CreatePATDetails.png" alt="Tableau PAT Details" ></div>

5. In the Migration app:
   1. Enter the **Token Name** in the **PAT Name** field.
   2. Enter the **Secret** that you copied earlier in the **PAT** field.

This needs to be done separately for both the source Tableau Server and the destination Tableau Cloud. Each PAT only provides access to the system it was generated on.

**Note** The PAT only provides access to resources the user is authorized to have. Resources that the PAT owner doesn’t have access to will **not** be migrated.

### User Mappings
> **!Important!** Users in Tableau Cloud must have a username in the form of an email. Because Tableau Server doesn’t have this restriction, we've provided different ways for you to designate how the users should be migrated if they don't currently follow this format. The different cases are detailed below.

The order of priority of defined mappings for the migration are:
1. CSV-defined mapping
2. Username already in email format
3. User has an associated email
4. Default domain mapping

This means that if a defined mapping is found for a user in the CSV file, that mapping is used even if the user has an associated email.

The following sections detail more about each mapping option.

#### Username already in email format
If a username is already in an email format, then nothing needs to be done and the user will be migrated over as is.

#### User has an associated email
If a user has an associated email set to the profile, that email will instead be used for the user when migrating.

#### Default Domain Mapping
> <div align="center"><img src="/screenshots/DefaultDomainMapping.png" alt="Tableau Migration App Token Fields" ></div>

We also provide a default domain mapping. When this is done, all users will have their names appended with that domain to create an email.

For example, if a user with username `PeterP` is to be migrated, and the `Default User Domain` is set to `DailyBugle.com`, then that user will be migrated to Tableau Cloud as `PeterP@DailyBugle.com`

This option can be disabled by clicking the checkbox. In this case, users that don’t have a mapping found from the other options will **not** be migrated to Tableau Cloud.

**Note** Users containing illegal email characters in their usernames will still fail migration even with a default domain mapped. For example `[]-=@example.com` isn’t a valid username for Tableau Cloud.

**Note2** Users with a space in their Tableau Server usernames such as `Renee Montoya` will have their spaces replaced with a `.` instead, becoming `Renee.Montoya@defaultDomain.here`.

#### CSV Mapping
> <div align="center"><img src="/screenshots/UserFileMapping.png" alt="Tableau Migration App Token Fields" ></div>
For more fine-grained control over how individual users are migrated, you can define specific user mappings in a CSV file.

To do so, the file should contain one user mapping per line, separated by a comma (`,`).

That is, every line should look like: `{TableauServerUserName},{TableauCloudUsername}`

The `{TableauServerUserName}` **MUST** match the username exactly and is **case-sensitive** or else the user will not be mapped.

Here’s an example of the proper file format:

<div style="border: 1px solid #000; padding: 10px; border-radius: 8px;">

`example.csv`:
```
Timmy Turner, TTurner@fairytale.net
Velma Dinkley, dinkley.velma@mysteries.com
Leonardo, blue@turtle.com
...
```
</div>
<br />

**Note** Starting and trailing white spaces are trimmed. `user1   ,   user1@domain.com` is the same as `user1,user1@domain.com` for the sake of legibility.

### Outputs
Ongoing outputs of the running Migration will be shown in the output window.
> <div align="center"><img src="/screenshots/OutputWindow.png" alt="Tableau Migration App Output" width=400 ></div>

After migration of a resource is completed, whether successful or not, an associated message shows in the window.

> <div align="center"><img src="/screenshots/OutputWindowFilled.png" alt="Tableau Migration App Output" width=400 ></div>
### Logging
Application logs can be found in the `Logs` folder from where the application is run. These logs contain all the REST requests made by the underlying [Tableau Migration SDK](https://github.com/tableau/tableau-migration-sdk) to the Tableau environments to perform the migration.

These logs can be used to provide further insight to the ongoing status and/or errors of the migration.

### Canceling a Migration
When a migration is running, a **Cancel Migration** button appears. If you want to stop the current migration, you can click this button. Doing so will also present you with a dialog window to save a manifest file.

> <div align="center"><img src="/screenshots/SaveManifest.png" alt="Tableau Migration App Save Manifest" width=400 ></div>

This manifest can be used to resume a migration later and continue where you left off.

### Resuming a Migration
If you have a manifest file saved from a previous run, you can resume a migration by selecting the dropdown arrow on the right side of the `Start Migration` button and selecting `Resume Migration`.

> <div align="center"><img src="/screenshots/Resume.png" alt="Tableau Migration App Output" width=400 ></div>


### FAQ

#### The application is taking a long time, is it stuck?!
> Check the logs. It could be that the current resource is large and is taking a long time to migrate.
>
> Or it could be that the application has hit its limit on the number of requests it can make in the current time frame, and is waiting until it can send more. In this case, the logs messages should mention how much longer it needs to wait for.

#### How do I migrate items with more specific logic rules?
> If you want finer control over how the migration is handled, such as renaming resources or applying special rules like reassigning resources, then we recommend that you use the [Tableau Migration SDK](https://github.com/tableau/tableau-migration-sdk) to programmatically perform the migration.

#### How do I exclude certain items from being migrated?
> You can either delete the items after the migration is performed, or you’ll have to use the [Tableau Migration SDK](https://github.com/tableau/tableau-migration-sdk)

#### What order will my resources get migrated in?
> Resources are first sorted from largest to smallest, and migrated in that order.
