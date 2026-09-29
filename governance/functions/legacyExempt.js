/**
 * Wraps the standard `truthy` check but exempts APIs tagged as legacy
 * (info.x-lifecycle === "legacy") from the rule entirely.
 *
 * Used to override specific rules from the shared, GitHub-hosted governance
 * ruleset for legacy APIs while leaving every other rule enforced as-is.
 */
module.exports = (targetVal, _opts, context) => {
  const document = context.document && context.document.data;
  const lifecycle = document && document.info && document.info['x-lifecycle'];

  if (lifecycle === 'legacy') {
    return [];
  }

  if (!targetVal) {
    return [
      {
        message: context.rule && context.rule.message,
      },
    ];
  }

  return [];
};
