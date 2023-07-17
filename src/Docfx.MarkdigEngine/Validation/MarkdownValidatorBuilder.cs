// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;

using Docfx.Common;
using Docfx.Plugins;

namespace Docfx.MarkdigEngine.Extensions;

public class MarkdownValidatorBuilder
{
    private readonly List<RuleWithId<MarkdownValidationRule>> _validators = new();
    private readonly List<RuleWithId<MarkdownTagValidationRule>> _tagValidators = new();
    private readonly Dictionary<string, MarkdownValidationRule> _globalValidators = new();
    private readonly List<MarkdownValidationSetting> _settings = new();

    public const string DefaultValidatorName = "default";
    public const string MarkdownValidatePhaseName = "Markdown style";

    public static MarkdownValidatorBuilder Create(MarkdownServiceParameters parameters)
    {
        var builder = new MarkdownValidatorBuilder();
        if (parameters != null)
        {
            LoadValidatorConfig(parameters.BasePath, parameters.TemplateDir, builder);
        }

        return builder;
    }

    public IMarkdownObjectRewriter CreateRewriter(MarkdownContext context)
    {
        return new TagValidator(GetEnabledTagRules().ToImmutableList(), context);
    }

    public void AddValidators(MarkdownValidationRule[] rules)
    {
        if (rules == null)
        {
            return;
        }
        foreach (var rule in rules)
        {
            if (string.IsNullOrEmpty(rule.ContractName))
            {
                continue;
            }
            _globalValidators[rule.ContractName] = rule;
        }
    }

    public void AddValidators(string category, Dictionary<string, MarkdownValidationRule> validators)
    {
        if (validators == null)
        {
            return;
        }
        foreach (var pair in validators)
        {
            if (string.IsNullOrEmpty(pair.Value.ContractName))
            {
                continue;
            }
            _validators.Add(new RuleWithId<MarkdownValidationRule>
            {
                Category = category,
                Id = pair.Key,
                Rule = pair.Value,
            });
        }
    }

    public void AddTagValidators(MarkdownTagValidationRule[] validators)
    {
        if (validators == null)
        {
            return;
        }

        foreach (var item in validators)
        {
            _tagValidators.Add(new RuleWithId<MarkdownTagValidationRule>
            {
                Category = null,
                Id = null,
                Rule = item
            });
        }
    }

    internal void AddTagValidators(string category, Dictionary<string, MarkdownTagValidationRule> validators)
    {
        if (validators == null)
        {
            return;
        }

        foreach (var pair in validators)
        {
            _tagValidators.Add(new RuleWithId<MarkdownTagValidationRule>
            {
                Category = category,
                Id = pair.Key,
                Rule = pair.Value,
            });
        }
    }

    internal void AddSettings(MarkdownValidationSetting[] settings)
    {
        if (settings == null)
        {
            return;
        }
        foreach (var setting in settings)
        {
            _settings.Add(setting);
        }
    }

    private void EnsureDefaultValidator()
    {
        if (!_globalValidators.ContainsKey(DefaultValidatorName))
        {
            _globalValidators[DefaultValidatorName] = new MarkdownValidationRule
            {
                ContractName = DefaultValidatorName
            };
        }
    }

    private static void LoadValidatorConfig(string baseDir, string templateDir, MarkdownValidatorBuilder builder)
    {
        if (string.IsNullOrEmpty(baseDir))
        {
            return;
        }

        if (templateDir != null)
        {
            var configFolder = Path.Combine(templateDir, MarkdownStyleDefinition.MarkdownStyleDefinitionFolderName);
            if (Directory.Exists(configFolder))
            {
                LoadValidatorDefinition(configFolder, builder);
            }
        }

        var configFile = Path.Combine(baseDir, MarkdownStyleConfig.MarkdownStyleFileName);
        if (EnvironmentContext.FileAbstractLayer.Exists(configFile))
        {
            var config = JsonUtility.Deserialize<MarkdownStyleConfig>(configFile);
            builder.AddValidators(config.Rules);
            builder.AddTagValidators(config.TagRules);
            builder.AddSettings(config.Settings);
        }
        builder.EnsureDefaultValidator();
    }

    private static void LoadValidatorDefinition(string mdStyleDefPath, MarkdownValidatorBuilder builder)
    {
        if (Directory.Exists(mdStyleDefPath))
        {
            foreach (var configFile in Directory.GetFiles(mdStyleDefPath, "*" + MarkdownStyleDefinition.MarkdownStyleDefinitionFilePostfix))
            {
                var fileName = Path.GetFileName(configFile);
                var category = fileName.Remove(fileName.Length - MarkdownStyleDefinition.MarkdownStyleDefinitionFilePostfix.Length);
                var config = JsonUtility.Deserialize<MarkdownStyleDefinition>(configFile);
                builder.AddTagValidators(category, config.TagRules);
                builder.AddValidators(category, config.Rules);
            }
        }
    }

    private IEnumerable<MarkdownTagValidationRule> GetEnabledTagRules()
    {
        foreach (var item in _tagValidators)
        {
            if (IsDisabledBySetting(item) ?? item.Rule.Disable)
            {
                continue;
            }
            yield return item.Rule;
        }
    }

    private bool? IsDisabledBySetting<T>(RuleWithId<T> item)
    {
        bool? categoryDisable = null;
        bool? idDisable = null;
        if (item.Category != null)
        {
            foreach (var setting in _settings)
            {
                if (setting.Category == item.Category)
                {
                    if (setting.Id == null)
                    {
                        categoryDisable = setting.Disable;
                    }
                    else if (setting.Id == item.Id)
                    {
                        idDisable = setting.Disable;
                    }
                }
            }
        }
        return idDisable ?? categoryDisable;
    }

    #region Nested Classes
    private sealed class RuleWithId<T>
    {
        public T Rule { get; set; }
        public string Category { get; set; }
        public string Id { get; set; }
    }
    #endregion
}