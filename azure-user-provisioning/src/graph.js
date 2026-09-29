const fs = require('fs');
const path = require('path');
const axios = require('axios');

const GRAPH_BASE_URL = 'https://graph.microsoft.com/v1.0';
const MEMBER_SELECT_FIELDS = 'id,displayName,mail,department';

/**
 * Returns the members of the configured Azure AD group.
 *
 * In mock mode this reads a local fixture so the flow can be demoed without a
 * live tenant. In live mode it authenticates with a client-credentials app
 * registration (AZURE_TENANT_ID/AZURE_CLIENT_ID/AZURE_CLIENT_SECRET, which
 * needs GroupMember.Read.All application permission, admin-consented) and
 * pages through Microsoft Graph's /groups/{id}/members endpoint.
 */
async function getGroupMembers({ mock, groupId, tenantId, clientId, clientSecret }) {
  if (mock) {
    const fixturePath = path.join(__dirname, '..', 'fixtures', 'aad-group-members.json');
    return JSON.parse(fs.readFileSync(fixturePath, 'utf-8'));
  }

  if (!groupId || !tenantId || !clientId || !clientSecret) {
    throw new Error(
      'Live mode requires AZURE_GROUP_ID, AZURE_TENANT_ID, AZURE_CLIENT_ID and AZURE_CLIENT_SECRET to be set.'
    );
  }

  // Lazy-require so mock mode doesn't need @azure/identity installed/configured.
  const { ClientSecretCredential } = require('@azure/identity');
  const credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
  const token = await credential.getToken('https://graph.microsoft.com/.default');

  const members = [];
  let url = `${GRAPH_BASE_URL}/groups/${groupId}/members?$select=${MEMBER_SELECT_FIELDS}`;

  while (url) {
    const response = await axios.get(url, {
      headers: { Authorization: `Bearer ${token.token}` },
    });
    members.push(...(response.data.value || []));
    url = response.data['@odata.nextLink'] || null;
  }

  return members;
}

module.exports = { getGroupMembers };
