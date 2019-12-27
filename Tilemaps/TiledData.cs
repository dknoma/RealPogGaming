using Tilemaps;
using static Tilemaps.TiledRenderOrder;

public class TiledData {
    private TiledInfo tiledInfo;

    [DisableInspectorEdit] 
    private RenderOrder renderOrder;

    public TiledData(TiledInfo tiledInfo) {
        this.tiledInfo = tiledInfo;
        this.renderOrder = GetRenderOrder(tiledInfo.renderorder);
    }
}
