# Azure AD -> SwaggerHub / Swagger Studio user provisioning

Automates the manual, slow step of inviting new users into SwaggerHub once
they're provisioned in Azure AD (Entra ID). Currently the SwaggerHub
User Management API endpoints exist but aren't wired up to anything - this
closes that gap.

## Flow

1. Read the members of a designated Azure AD group (`AZURE_GROUP_ID`) via
   Microsoft Graph (`GET /groups/{id}/members`).
2. Diff against a local state file (`state/synced-users.json`) so already
   provisioned users aren't re-invited on every run.
3. For each new member:
   - `POST /orgs/{org}/members` - invite them into the SwaggerHub organization.
   - `POST /orgs/{org}/resources/{org}/resource-type/ORGANIZATION/users` -
     grant them a role (ADMIN / DESIGNER / CONSUMER), derived from their AAD
     `department` attribute via `src/roleMap.js` (swap for whatever
     attribute/app-role your tenant actually uses).

## Running the demo tonight (no live credentials needed)

```
cd azure-user-provisioning
npm install
npm run start:mock
```

This reads `fixtures/aad-group-members.json` instead of calling Graph, and
logs what it *would* send to SwaggerHub instead of calling the live API. Run
it twice to see the second run correctly skip already-provisioned users.

## Switching to live tomorrow

1. Copy `.env.example` to `.env` and fill in:
   - `AZURE_TENANT_ID` / `AZURE_CLIENT_ID` / `AZURE_CLIENT_SECRET` - an Azure
     AD app registration granted the `GroupMember.Read.All` **application**
     permission (admin consent required).
   - `AZURE_GROUP_ID` - the object ID of the AAD group whose members should
     get SwaggerHub access.
   - `SWAGGERHUB_API_KEY` - an API key for a SwaggerHub org owner/admin
     (Account Settings -> API Key).
   - `SWAGGERHUB_ORG` - the SwaggerHub org name (e.g. `domcrosbie-cc0`).
2. Set `MOCK=false`.
3. `npm start`.

## Wiring it up to trigger automatically

For the demo this runs on demand from the command line. To make it
"automatic" in production, either:

- Deploy `src/index.js` as an **Azure Function** with a Timer trigger (e.g.
  every 15 minutes) that polls the group for changes, or
- Use a Microsoft Graph **change notification (webhook)** subscription on the
  group's membership, delivered to an Azure Function HTTP trigger, so new
  members are provisioned within seconds instead of on a poll interval.

Both call the exact same `src/graph.js` / `src/swaggerhub.js` functions used
here - only the trigger mechanism changes.
