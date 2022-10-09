﻿using System;
using System.Threading.Tasks;
using ChainFx;
using ChainFx.Web;
using static ChainFx.Fabric.Nodality;

namespace ChainMart
{
    public abstract class RptWork : WebWork
    {
    }

    [UserAuthorize(admly: User.ADMLY_)]
    [Ui("供给业务报表", "业务")]
    public class AdmlyPrvRptWork : RptWork
    {
        public void @default(WebContext wc, int page)
        {
            wc.GivePage(200, h => { h.TOOLBAR(); });
        }
    }

    [UserAuthorize(admly: User.ADMLY_)]
    [Ui("消费业务报表", "业务")]
    public class AdmlyBuyRptWork : RptWork
    {
        public void @default(WebContext wc, int page)
        {
            wc.GivePage(200, h => { h.TOOLBAR(); });
        }
    }

    [UserAuthorize(Org.TYP_ZON, User.ORGLY_)]
    [Ui("综合报表", "供区")]
    public class ZonlyRptWork : RptWork
    {
        public void @default(WebContext wc, int page)
        {
            var org = wc[-1].As<Org>();
            using var dc = NewDbContext();
            dc.Sql("SELECT * FROM dailys WHERE orgid = @1 ORDER BY dt DESC LIMIT 30 OFFSET 30 * @2");
            var arr = dc.Query<Rptie>(p => p.Set(org.id).Set(page));
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.TABLE(arr, o =>
                {
                    h.TD(Rptie.Typs[o.typ]);
                    h.TD_().T(o.dt, 3, 0)._TD();
                    h.TD_().T(o.count).SP().T('笔')._TD();
                    h.TD(o.amt, currency: true);
                });
                h.PAGINATION(arr?.Length == 30);
            }, false, 3);
        }

        [UserAuthorize(Org.TYP_ZON, User.ORGLY_FIN)]
        [Ui("生成"), Tool(Modal.ButtonOpen)]
        public async Task gen(WebContext wc, int page)
        {
            var prin = (User) wc.Principal;
            short orgid = wc[-1];
            DateTime date;
            short typ = 0;
            decimal amt = 0;
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDSUL_("指定统计区间");
                    h.LI_().DATE("从日期", nameof(date), DateTime.Today, required: true)._LI();
                    h.LI_().DATE("到日期", nameof(date), DateTime.Today, required: true)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                var f = await wc.ReadAsync<Form>();
                date = f[nameof(date)];
                date = f[nameof(date)];
                wc.GivePane(200); // close dialog
            }
        }
    }


    [UserAuthorize(Org.TYP_SRC, User.ORGLY_LOG)]
    [Ui("业务报表", "产源")]
    public class SrclyRptWork : BookWork
    {
        public async Task @default(WebContext wc, int page)
        {
        }
    }

    [UserAuthorize(Org.TYP_DST, User.ORGLY_)]
    [Ui("业务报表", "中控")]
    public class CtrlyRptWork : RptWork
    {
        [Ui("待收", @group: 1), Tool(Modal.Anchor)]
        public async Task @default(WebContext wc, int page)
        {
        }
    }

    [UserAuthorize(Org.TYP_SHP, User.ORGLY_)]
    [Ui("线上业务报表", "商户")]
    public class ShplyRptWork : RptWork
    {
        [Ui("待收", group: 1), Tool(Modal.Anchor)]
        public async Task @default(WebContext wc, int page)
        {
        }
    }
}