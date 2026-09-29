/**
 * Maps an Azure AD attribute (department) to a SwaggerHub organization role.
 * Adjust the mapping to whatever AAD attribute/app role your tenant actually
 * uses to signal SwaggerHub access level (e.g. an app role assignment or a
 * custom extension attribute instead of `department`).
 */
const DEPARTMENT_TO_ROLE = {
  admin: 'ADMIN',
  designer: 'DESIGNER',
};

const DEFAULT_ROLE = 'CONSUMER';

function mapDepartmentToRole(department) {
  if (!department) return DEFAULT_ROLE;
  return DEPARTMENT_TO_ROLE[department.toLowerCase()] || DEFAULT_ROLE;
}

module.exports = { mapDepartmentToRole, DEFAULT_ROLE };
