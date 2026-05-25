using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace ReclaimCS.Shared.Menus;

public sealed class CenterHtmlHintFlashFixService
{
    private CCSGameRules? _gameRules;

    public void OnMapStart()
    {
        InitializeGameRules();
    }

    public void Update()
    {
        if (_gameRules == null)
        {
            InitializeGameRules();
            return;
        }

        _gameRules.GameRestart = _gameRules.RestartRoundTime < Server.CurrentTime;
    }

    private void InitializeGameRules()
    {
        var gameRulesProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault();
        _gameRules = gameRulesProxy?.GameRules;
    }
}
