require('dotenv').config();

const { getGroupMembers } = require('./graph');
const { inviteMember, grantOrganizationRole } = require('./swaggerhub');
const { mapDepartmentToRole } = require('./roleMap');
const { readState, writeState } = require('./state');

const MOCK = String(process.env.MOCK).toLowerCase() === 'true';
const STATE_FILE = process.env.STATE_FILE || './state/synced-users.json';

async function main() {
  const config = {
    mock: MOCK,
    groupId: process.env.AZURE_GROUP_ID,
    tenantId: process.env.AZURE_TENANT_ID,
    clientId: process.env.AZURE_CLIENT_ID,
    clientSecret: process.env.AZURE_CLIENT_SECRET,
  };

  const swaggerHubOrg = process.env.SWAGGERHUB_ORG;
  const apiKey = process.env.SWAGGERHUB_API_KEY;
  const userType = process.env.SWAGGERHUB_USER_TYPE || 'INTERNAL';

  console.log(`\n[1/3] Fetching Azure AD group members${MOCK ? ' (mock fixture)' : ' (live Microsoft Graph)'}...`);
  const members = await getGroupMembers(config);
  console.log(`      Found ${members.length} member(s) in the group.`);

  const state = readState(STATE_FILE);
  const alreadyProvisioned = new Set(state.provisioned);
  const newMembers = members.filter((m) => m.mail && !alreadyProvisioned.has(m.mail));

  console.log(`\n[2/3] ${newMembers.length} new user(s) to provision in SwaggerHub org "${swaggerHubOrg}".`);

  const results = [];
  for (const member of newMembers) {
    const role = mapDepartmentToRole(member.department);

    await inviteMember({ org: swaggerHubOrg, email: member.mail, userType, apiKey, mock: MOCK });
    await grantOrganizationRole({ org: swaggerHubOrg, email: member.mail, role, userType, apiKey, mock: MOCK });

    alreadyProvisioned.add(member.mail);
    results.push({ name: member.displayName, email: member.mail, role });
    console.log(`      -> Invited ${member.displayName} <${member.mail}> as ${role}`);
  }

  writeState(STATE_FILE, { provisioned: [...alreadyProvisioned] });

  console.log('\n[3/3] Done.');
  console.table(results.length ? results : [{ name: '(none)', email: '', role: '' }]);
}

main().catch((err) => {
  console.error('Provisioning run failed:', err.message);
  process.exit(1);
});
