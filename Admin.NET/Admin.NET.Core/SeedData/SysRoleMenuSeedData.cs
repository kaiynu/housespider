namespace Admin.NET.Core;

/// <summary>
/// 系统角色菜单表种子数据
/// </summary>
public class SysRoleMenuSeedData : ISqlSugarEntitySeedData<SysRoleMenu>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysRoleMenu> HasData()
    {
        return new[]
        {
            // 数据面板【admin/252885263003721】
            new SysRoleMenu{ Id=252885263003000, RoleId=252885263003721, MenuId=252885263002100 },
            new SysRoleMenu{ Id=252885263003001, RoleId=252885263003721, MenuId=252885263002110 },
            new SysRoleMenu{ Id=252885263003002, RoleId=252885263003721, MenuId=252885263002111 },

            // 系统管理
            new SysRoleMenu{ Id=252885263003100, RoleId=252885263003721, MenuId=252885263002200 },
            // 账号管理
            new SysRoleMenu{ Id=252885263003101, RoleId=252885263003721, MenuId=252885263002210 },
            new SysRoleMenu{ Id=252885263003102, RoleId=252885263003721, MenuId=252885263002211 },
            new SysRoleMenu{ Id=252885263003103, RoleId=252885263003721, MenuId=252885263002212 },
            new SysRoleMenu{ Id=252885263003104, RoleId=252885263003721, MenuId=252885263002213 },
            new SysRoleMenu{ Id=252885263003105, RoleId=252885263003721, MenuId=252885263002214 },
            new SysRoleMenu{ Id=252885263003106, RoleId=252885263003721, MenuId=252885263002215 },
            new SysRoleMenu{ Id=252885263003107, RoleId=252885263003721, MenuId=252885263002216 },
            new SysRoleMenu{ Id=252885263003108, RoleId=252885263003721, MenuId=252885263002217 },
            new SysRoleMenu{ Id=252885263003109, RoleId=252885263003721, MenuId=252885263002218 },
            new SysRoleMenu{ Id=252885263003110, RoleId=252885263003721, MenuId=252885263002219 },
            new SysRoleMenu{ Id=252885263013111, RoleId=252885263003721, MenuId=252885263012220 },
            // 角色管理
            new SysRoleMenu{ Id=252885263003111, RoleId=252885263003721, MenuId=252885263002220 },
            new SysRoleMenu{ Id=252885263003112, RoleId=252885263003721, MenuId=252885263002221 },
            new SysRoleMenu{ Id=252885263003113, RoleId=252885263003721, MenuId=252885263002222 },
            new SysRoleMenu{ Id=252885263003114, RoleId=252885263003721, MenuId=252885263002223 },
            new SysRoleMenu{ Id=252885263003115, RoleId=252885263003721, MenuId=252885263002224 },
            new SysRoleMenu{ Id=252885263003116, RoleId=252885263003721, MenuId=252885263002225 },
            new SysRoleMenu{ Id=252885263003117, RoleId=252885263003721, MenuId=252885263002226 },
            new SysRoleMenu{ Id=252885263003118, RoleId=252885263003721, MenuId=252885263002227 },
            // 菜单管理
            new SysRoleMenu{ Id=252885263003121, RoleId=252885263003721, MenuId=252885263002230 },
            new SysRoleMenu{ Id=252885263003122, RoleId=252885263003721, MenuId=252885263002231 },
            new SysRoleMenu{ Id=252885263003123, RoleId=252885263003721, MenuId=252885263002232 },
            new SysRoleMenu{ Id=252885263003124, RoleId=252885263003721, MenuId=252885263002233 },
            new SysRoleMenu{ Id=252885263003125, RoleId=252885263003721, MenuId=252885263002234 },
            // 机构管理
            new SysRoleMenu{ Id=252885263003131, RoleId=252885263003721, MenuId=252885263002240 },
            new SysRoleMenu{ Id=252885263003132, RoleId=252885263003721, MenuId=252885263002241 },
            new SysRoleMenu{ Id=252885263003133, RoleId=252885263003721, MenuId=252885263002242 },
            new SysRoleMenu{ Id=252885263003134, RoleId=252885263003721, MenuId=252885263002243 },
            new SysRoleMenu{ Id=252885263003135, RoleId=252885263003721, MenuId=252885263002244 },
            // 职位管理
            new SysRoleMenu{ Id=252885263003141, RoleId=252885263003721, MenuId=252885263002250 },
            new SysRoleMenu{ Id=252885263003142, RoleId=252885263003721, MenuId=252885263002251 },
            new SysRoleMenu{ Id=252885263003143, RoleId=252885263003721, MenuId=252885263002252 },
            new SysRoleMenu{ Id=252885263003144, RoleId=252885263003721, MenuId=252885263002253 },
            new SysRoleMenu{ Id=252885263003145, RoleId=252885263003721, MenuId=252885263002254 },
            // 个人中心
            new SysRoleMenu{ Id=252885263003151, RoleId=252885263003721, MenuId=252885263002260 },
            new SysRoleMenu{ Id=252885263003152, RoleId=252885263003721, MenuId=252885263002261 },
            new SysRoleMenu{ Id=252885263003153, RoleId=252885263003721, MenuId=252885263002262 },
            new SysRoleMenu{ Id=252885263003154, RoleId=252885263003721, MenuId=252885263002263 },
            // 通知公告
            new SysRoleMenu{ Id=252885263003161, RoleId=252885263003721, MenuId=252885263002270 },
            new SysRoleMenu{ Id=252885263003162, RoleId=252885263003721, MenuId=252885263002271 },
            new SysRoleMenu{ Id=252885263003163, RoleId=252885263003721, MenuId=252885263002272 },
            new SysRoleMenu{ Id=252885263003164, RoleId=252885263003721, MenuId=252885263002273 },
            new SysRoleMenu{ Id=252885263003165, RoleId=252885263003721, MenuId=252885263002274 },
            new SysRoleMenu{ Id=252885263003166, RoleId=252885263003721, MenuId=252885263002275 },
            new SysRoleMenu{ Id=252885263003167, RoleId=252885263003721, MenuId=252885263002276 },

            // 平台管理
            new SysRoleMenu{ Id=252885263003200, RoleId=252885263003721, MenuId=252885263002300 },
            // 参数配置
            new SysRoleMenu{ Id=252885263003201, RoleId=252885263003721, MenuId=252885263002320 },
            new SysRoleMenu{ Id=252885263003202, RoleId=252885263003721, MenuId=252885263002321 },
            new SysRoleMenu{ Id=252885263003203, RoleId=252885263003721, MenuId=252885263002322 },
            new SysRoleMenu{ Id=252885263003204, RoleId=252885263003721, MenuId=252885263002323 },
            new SysRoleMenu{ Id=252885263003205, RoleId=252885263003721, MenuId=252885263002324 },
            // 字典管理
            new SysRoleMenu{ Id=252885263003211, RoleId=252885263003721, MenuId=252885263002330 },
            new SysRoleMenu{ Id=252885263003212, RoleId=252885263003721, MenuId=252885263002331 },
            new SysRoleMenu{ Id=252885263003213, RoleId=252885263003721, MenuId=252885263002332 },
            new SysRoleMenu{ Id=252885263003214, RoleId=252885263003721, MenuId=252885263002333 },
            new SysRoleMenu{ Id=252885263003215, RoleId=252885263003721, MenuId=252885263002334 },
            // 系统监控
            new SysRoleMenu{ Id=252885263003231, RoleId=252885263003721, MenuId=252885263002360 },
            // 缓存管理
            new SysRoleMenu{ Id=252885263003241, RoleId=252885263003721, MenuId=252885263002370 },
            new SysRoleMenu{ Id=252885263003242, RoleId=252885263003721, MenuId=252885263002371 },
            new SysRoleMenu{ Id=252885263003243, RoleId=252885263003721, MenuId=252885263002372 },
            // 行政区域
            new SysRoleMenu{ Id=252885263003251, RoleId=252885263003721, MenuId=252885263002380 },
            new SysRoleMenu{ Id=252885263003252, RoleId=252885263003721, MenuId=252885263002381 },
            new SysRoleMenu{ Id=252885263003253, RoleId=252885263003721, MenuId=252885263002382 },
            new SysRoleMenu{ Id=252885263003254, RoleId=252885263003721, MenuId=252885263002383 },
            new SysRoleMenu{ Id=252885263003255, RoleId=252885263003721, MenuId=252885263002384 },
            new SysRoleMenu{ Id=252885263003256, RoleId=252885263003721, MenuId=252885263002385 },
            // 文件管理
            new SysRoleMenu{ Id=252885263003261, RoleId=252885263003721, MenuId=252885263002390 },
            new SysRoleMenu{ Id=252885263003262, RoleId=252885263003721, MenuId=252885263002391 },
            new SysRoleMenu{ Id=252885263003263, RoleId=252885263003721, MenuId=252885263002392 },
            new SysRoleMenu{ Id=252885263003264, RoleId=252885263003721, MenuId=252885263002393 },
            new SysRoleMenu{ Id=252885263003265, RoleId=252885263003721, MenuId=252885263002394 },

            // 日志管理
            new SysRoleMenu{ Id=252885263003300, RoleId=252885263003721, MenuId=252885263002500 },
            new SysRoleMenu{ Id=252885263003301, RoleId=252885263003721, MenuId=252885263002510 },
            new SysRoleMenu{ Id=252885263003302, RoleId=252885263003721, MenuId=252885263002511 },
            new SysRoleMenu{ Id=252885263003311, RoleId=252885263003721, MenuId=252885263002520 },
            new SysRoleMenu{ Id=252885263003312, RoleId=252885263003721, MenuId=252885263002521 },
            new SysRoleMenu{ Id=252885263003321, RoleId=252885263003721, MenuId=252885263002530 },
            new SysRoleMenu{ Id=252885263003322, RoleId=252885263003721, MenuId=252885263002531 },
            new SysRoleMenu{ Id=252885263003331, RoleId=252885263003721, MenuId=252885263002540 },
            new SysRoleMenu{ Id=252885263003332, RoleId=252885263003721, MenuId=252885263002541 },

            // 帮助文档
            new SysRoleMenu{ Id=252885263003500, RoleId=252885263003721, MenuId=252885263002700 },
            new SysRoleMenu{ Id=252885263003501, RoleId=252885263003721, MenuId=252885263002710 },
            new SysRoleMenu{ Id=252885263003502, RoleId=252885263003721, MenuId=252885263002711 },

            // 数据面板【user1/252885263003722】
            new SysRoleMenu{ Id=252885263004000, RoleId=252885263003722, MenuId=252885263002100 },
            new SysRoleMenu{ Id=252885263004001, RoleId=252885263003722, MenuId=252885263002110 },
            new SysRoleMenu{ Id=252885263004002, RoleId=252885263003722, MenuId=252885263002111 },
            // 系统管理
            new SysRoleMenu{ Id=252885263004100, RoleId=252885263003722, MenuId=252885263002200 },
            // 个人中心
            new SysRoleMenu{ Id=252885263004151, RoleId=252885263003722, MenuId=252885263002260 },
            new SysRoleMenu{ Id=252885263004152, RoleId=252885263003722, MenuId=252885263002261 },
            new SysRoleMenu{ Id=252885263004153, RoleId=252885263003722, MenuId=252885263002262 },
            new SysRoleMenu{ Id=252885263004154, RoleId=252885263003722, MenuId=252885263002263 },

            // 数据面板【user3/252885263003724】
            new SysRoleMenu{ Id=252885263005000, RoleId=252885263003724, MenuId=252885263002100 },
            new SysRoleMenu{ Id=252885263005001, RoleId=252885263003724, MenuId=252885263002110 },
            new SysRoleMenu{ Id=252885263005002, RoleId=252885263003724, MenuId=252885263002111 },
            // 系统管理
            new SysRoleMenu{ Id=252885263005100, RoleId=252885263003724, MenuId=252885263002200 },
            // 个人中心
            new SysRoleMenu{ Id=252885263005151, RoleId=252885263003724, MenuId=252885263002260},
            new SysRoleMenu{ Id=252885263005152, RoleId=252885263003724, MenuId=252885263002261 },
            new SysRoleMenu{ Id=252885263005153, RoleId=252885263003724, MenuId=252885263002262 },
            new SysRoleMenu{ Id=252885263005154, RoleId=252885263003724, MenuId=252885263002263 },
        };
    }
}