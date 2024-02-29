using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using GameData.Domains.Item;
using HarmonyLib;
using GameData.Domains.Item.Display;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace LittleBlackSnakeModify
{
    [PluginConfig("LittleBlackSnakeModify", "在下炮灰", "0.1.0")]
    public class BackendPatch: TaiwuRemakePlugin
    {
        Harmony harmony;
        public override void Dispose()
        {
            if(harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            harmony = new Harmony("com.paohui.mod");
            harmony.PatchAll();

            AdaptableLog.Info("小黑蛇数值调整 后端Mod 启动了");
        }
    }

    [HarmonyPatch(typeof(ItemDomain))]
    [HarmonyPatch(nameof(ItemDomain.GetTaiwuInventoryCombatSkillBooks))]//这只是前端用来判断历练值是否够用的方法
    public class PatchGalbalConfigBackend
    {
        public static void Postfix(ref List<SkillBookModifyDisplayData> __result)//直接修改返回值, 使得所有修改书的历练消耗为原来的1/4。
        {
            foreach (SkillBookModifyDisplayData　b　in __result)
            {
                b.CostExp = b.CostExp / 4;
            }
        }
    }
    [HarmonyPatch(typeof(ItemDomain))]
    [HarmonyPatch(nameof(ItemDomain.ModifyCombatSkillBookPageNormal))]
    //这个函数包含了判断历练是否足够以及消耗历练完成修改操作
    public class PatchGalbalConfigBackend2
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode.Value == 0x1F && (sbyte)instruction.operand == 20)
                {
                    yield return new CodeInstruction(instruction.opcode, 5);//将乘以20替换为乘以5
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

}
