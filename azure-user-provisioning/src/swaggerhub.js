const axios = require('axios');

const BASE_URL = 'https://api.swaggerhub.com/user-management/v1';

/**
 * Invites a user into a SwaggerHub organization by email.
 * https://api.swaggerhub.com/user-management/v1/orgs/{org}/members
 */
async function inviteMember({ org, email, userType, apiKey, mock }) {
  if (mock) {
    return { mocked: true, action: 'invite', org, email, userType };
  }

  const response = await axios.post(
    `${BASE_URL}/orgs/${encodeURIComponent(org)}/members`,
    { members: [{ email }] },
    {
      params: { userType },
      headers: { Authorization: apiKey, 'Content-Type': 'application/json' },
    }
  );
  return response.data;
}

/**
 * Grants a user a specific role at the organization level.
 * https://api.swaggerhub.com/user-management/v1/orgs/{org}/resources/{org}/resource-type/ORGANIZATION/users
 */
async function grantOrganizationRole({ org, email, role, userType, apiKey, mock }) {
  if (mock) {
    return { mocked: true, action: 'grantRole', org, email, role, userType };
  }

  const response = await axios.post(
    `${BASE_URL}/orgs/${encodeURIComponent(org)}/resources/${encodeURIComponent(org)}/resource-type/ORGANIZATION/users`,
    { users: [{ email, role, userType: userType.toLowerCase() }] },
    { headers: { Authorization: apiKey, 'Content-Type': 'application/json' } }
  );
  return response.data;
}

module.exports = { inviteMember, grantOrganizationRole };
