using System.Threading.Tasks;
using SkyChain;
using SkyChain.Web;
using Zhnt;
using static SkyChain.Web.Modal;

namespace Zhnt
{
    [UserAuthorize(admly: User.ADMLY_PROD)]
    [Ui("品类")]
    public class AdmlyDItemWork : WebWork
    {
        protected override void OnMake()
        {
            MakeVarWork<AdmlyItemVarWork>();
        }

        public void @default(WebContext wc, int page)
        {
            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items ORDER BY typ, status DESC, id LIMIT 40 OFFSET 40 * @1");
            var arr = dc.Query<Item>(p => p.Set(page));
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.TABLE_();
                short last = 0;
                foreach (var o in arr)
                {
                    if (o.typ != last)
                    {
                        h.TR_().TD_("uk-label uk-padding-tiny-left", colspan: 6).T(Item.Typs[o.typ])._TD()._TR();
                    }
                    h.TR_();
                    h.TDCHECK(o.id);
                    h.TD_().VARTOOL(o.Key, nameof(AdmlyItemVarWork.upd), caption: o.name);
                    if (o.progg > 0) h.SP().SUB(Item.Progg[o.progg]);
                    h._TD();
                    h.TD_().CNY(o.price)._TD();
                    h.TD(Item.Statuses[o.status]);
                    h.TD_("uk-visible@l").T(o.tip)._TD();
                    h.TDFORM(() => h.VARTOOLS(o.Key));
                    h._TR();
                    last = o.typ;
                }
                h._TABLE();
                h.PAGINATION(arr.Length == 40);
            });
        }

        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            if (wc.IsGet)
            {
                var o = new Item();
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDSUL_("标品属性");
                    h.LI_().SELECT("类别", nameof(o.typ), o.typ, Item.Typs)._LI();
                    h.LI_().TEXT("标品名称", nameof(o.name), o.name, max: 10, required: true)._LI();
                    h.LI_().TEXT("亮点", nameof(o.tip), o.tip, max: 10)._LI();
                    h.LI_().SELECT("方案关联", nameof(o.progg), o.progg, Item.Progg)._LI();
                    h.LI_().NUMBER("价格", nameof(o.price), o.price, max: 500.00M, min: 0.00M, required: true)._LI();
                    h.LI_().SELECT("状态", nameof(o.status), o.status, Item.Statuses)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                var o = await wc.ReadObjectAsync<Item>(0);
                using var dc = NewDbContext();
                dc.Sql("INSERT INTO items ").colset(Item.Empty, 0)._VALUES_(Item.Empty, 0);
                await dc.ExecuteAsync(p => o.Write(p, 0));
                wc.GivePane(200); // close dialog
            }
        }
    }
}