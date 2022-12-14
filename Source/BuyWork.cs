using System.Threading.Tasks;
using ChainFx;
using ChainFx.Web;
using static ChainFx.Fabric.Nodality;
using static ChainFx.Web.Modal;
using static ChainMart.Notice;

namespace ChainMart
{
    public abstract class BuyWork<V> : WebWork where V : BuyVarWork, new()
    {
        protected override void OnCreate()
        {
            CreateVarWork<V>();
        }
    }

    [Ui("我的消费", "账号")]
    public class MyBuyWork : BuyWork<MyBuyVarWork>
    {
        public async Task @default(WebContext wc, int page)
        {
            var prin = (User) wc.Principal;

            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Buy.Empty).T(" FROM buys WHERE uid = @1 AND status > 0 ORDER BY id DESC LIMIT 10 OFFSET 10 * @2");
            var arr = await dc.QueryAsync<Buy>(p => p.Set(prin.id).Set(page));

            wc.GivePage(200, h =>
            {
                h.TOOLBAR(tip: prin.name);

                if (arr == null)
                {
                    h.ALERT("尚无消费订单");
                    return;
                }

                h.MAINGRID(arr, o =>
                {
                    h.HEADER_("uk-card-header").H4(o.name)._HEADER();
                    h.UL_("uk-card-body uk-list uk-list-divider");
                    h.LI_().T(o.created)._LI();
                    for (int i = 0; i < o?.details.Length; i++)
                    {
                        var ln = o.details[i];
                        h.LI_();
                        if (ln.itemid > 0)
                        {
                            h.PIC("/item/", ln.itemid, "/icon", css: "uk-width-micro");
                        }
                        else
                        {
                            h.PIC("/ware/", ln.itemid, "/icon", css: "uk-width-micro");
                        }
                        h.SPAN(ln.name, "uk-width-expand");
                        h.SPAN(ln.qty, "uk-width-1-6");
                        h._LI();
                    }
                    h._UL();
                    h.FOOTER_("uk-card-footer").T("合计：").T(o.topay)._FOOTER();
                });

                h.PAGINATION(arr?.Length > 10);
            });
        }
    }


    [OrglyAuthorize(Org.TYP_SHP, 1)]
    [Ui("消费订单", "商户")]
    public class ShplyBuyWork : BuyWork<ShplyBuyVarWork>
    {
        static void MainGrid(HtmlBuilder h, Buy[] arr)
        {
            h.MAINGRID(arr, o =>
            {
                h.ADIALOG_(o.Key, "/", ToolAttribute.MOD_OPEN, false, tip: o.uname, css: "uk-card-body uk-flex");

                // first detail line
                var first = o.details[0];
                if (first.itemid > 0)
                {
                    h.PIC_("uk-width-1-5").T(MainApp.WwwUrl).T("/item/").T(first.itemid).T("/icon")._PIC();
                }
                else
                {
                    h.PIC_("uk-width-1-5").T(MainApp.WwwUrl).T("/ware/").T(first.itemid).T("/icon")._PIC();
                }

                h.ASIDE_();
                h.HEADER_().H4(o.uname).SPAN_("uk-badge").T(o.created, time: 0).SP().T(Book.Statuses[o.status])._SPAN()._HEADER();
                h.Q(o.uaddr, "uk-width-expand");
                h.FOOTER_().SPAN_("uk-width-1-3")._SPAN().SPAN_("uk-width-1-3").T(o.details.Length).SP().T("项商品")._SPAN().SPAN_("uk-margin-auto-left").CNY(o.pay)._SPAN()._FOOTER();
                h._ASIDE();

                h._A();
            });
        }


        [BizNotice(BUY_CREATED)]
        [Ui("消费订单", group: 1), Tool(Anchor)]
        public async Task @default(WebContext wc)
        {
            var org = wc[-1].As<Org>();

            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Buy.Empty).T(" FROM buys WHERE shpid = @1 AND status = 1 ORDER BY id DESC");
            var arr = await dc.QueryAsync<Buy>(p => p.Set(org.id));

            wc.GivePage(200, h =>
            {
                h.TOOLBAR(notice: org.id);
                if (arr == null)
                {
                    h.ALERT("尚无网单");
                    return;
                }

                MainGrid(h, arr);
            });
        }

        [Ui(tip: "已发货", icon: "sign-out", group: 2), Tool(Anchor)]
        public async Task adapted(WebContext wc)
        {
            var org = wc[-1].As<Org>();

            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Buy.Empty).T(" FROM buys WHERE shpid = @1 AND status = 2 ORDER BY id DESC");
            var arr = await dc.QueryAsync<Buy>(p => p.Set(org.id));

            wc.GivePage(200, h =>
            {
                h.TOOLBAR(notice: org.id);
                if (arr == null)
                {
                    h.ALERT("尚无发货");
                    return;
                }

                MainGrid(h, arr);
            });
        }

        [BizNotice(BUY_OKED)]
        [Ui(tip: "已收货", icon: "sign-in", group: 4), Tool(Anchor)]
        public async Task oked(WebContext wc)
        {
            var org = wc[-1].As<Org>();

            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Buy.Empty).T(" FROM buys WHERE shpid = @1 AND status = 4 ORDER BY id DESC");
            var arr = await dc.QueryAsync<Buy>(p => p.Set(org.id));

            wc.GivePage(200, h =>
            {
                h.TOOLBAR(notice: org.id);
                if (arr == null)
                {
                    h.ALERT("尚无收货");
                    return;
                }

                MainGrid(h, arr);
            });
        }

        [Ui(tip: "已撤单", icon: "trash", group: 8), Tool(Anchor)]
        public async Task aborted(WebContext wc)
        {
            var org = wc[-1].As<Org>();

            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Buy.Empty).T(" FROM buys WHERE shpid = @1 AND status = 8 ORDER BY id DESC");
            var arr = await dc.QueryAsync<Buy>(p => p.Set(org.id));

            wc.GivePage(200, h =>
            {
                h.TOOLBAR(notice: org.id);
                if (arr == null)
                {
                    h.ALERT("尚无撤单");
                    return;
                }

                MainGrid(h, arr);
            });
        }
    }

    [OrglyAuthorize(Org.TYP_MKT, 1)]
    [Ui("消费订单统一送货", "盟主")]
    public class MktlyBuyWork : BuyWork<MktlyBuyVarWork>
    {
        [Ui("消费订单", group: 1), Tool(Anchor)]
        public async Task @default(WebContext wc)
        {
            var mkt = wc[-1].As<Org>();

            using var dc = NewDbContext();
            const short msk = Entity.MSK_EXTRA;
            dc.Sql("SELECT ").collst(Buy.Empty, msk).T(" FROM buys WHERE mktid = @1 AND state >= 0 ORDER BY uid DESC");
            var arr = await dc.QueryAsync<Buy>(p => p.Set(mkt.id), msk);

            wc.GivePage(200, h =>
            {
                h.TOOLBAR();

                h.MAIN_(grid: true);

                int last = 0; // uid

                foreach (var o in arr)
                {
                    if (o.uid != last)
                    {
                        h.FORM_("uk-card uk-card-default");
                        h.HEADER_("uk-card-header").T(o.uname).SP().T(o.utel).SP().T(o.uaddr)._HEADER();
                        h.UL_("uk-card-body");
                        // h.TR_().TD_("uk-label uk-padding-tiny-left", colspan: 6).T(spr.name)._TD()._TR();
                    }
                    h.LI_().T(o.name)._LI();

                    last = o.uid;
                }
                h._UL();
                h._FORM();

                h._MAIN();
            });
        }

        [Ui(tip: "历史", icon: "history", group: 2), Tool(Anchor)]
        public async Task past(WebContext wc)
        {
            var mkt = wc[-1].As<Org>();

            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Buy.Empty).T(" FROM buys WHERE mktid = @1 AND state >= 0 ORDER BY uid DESC");
            var arr = await dc.QueryAsync<Buy>(p => p.Set(mkt.id));

            wc.GivePage(200, h =>
            {
                h.TOOLBAR();

                h.MAIN_(grid: true);

                int last = 0; // uid

                foreach (var o in arr)
                {
                    if (o.uid != last)
                    {
                        h.FORM_("uk-card uk-card-default");
                        h.HEADER_("uk-card-header").T(o.uname).SP().T(o.utel).SP().T(o.uaddr)._HEADER();
                        h.UL_("uk-card-body");
                        // h.TR_().TD_("uk-label uk-padding-tiny-left", colspan: 6).T(spr.name)._TD()._TR();
                    }
                    h.LI_().T(o.name)._LI();

                    last = o.uid;
                }
                h._UL();
                h._FORM();

                h._MAIN();
            });
        }
    }
}