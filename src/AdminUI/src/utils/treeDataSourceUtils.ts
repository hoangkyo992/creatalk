import "./stringUtils";

class TreeDataSourceUtils {
  toNodes(data: any[], field: string) {
    const allNodes = data
      .map((o) => {
        return {
          key: o.id,
          type: "node",
          label: o[field],
          data: o,
          parentId: o.parentId,
          children: []
        };
      })
      .sort((a, b) => a.label.localeCompare(b.label));
    return this._toNodes(allNodes.filter((o) => !o.parentId).concat(allNodes.filter((o) => o.parentId)));
  }
  _toNodes(array) {
    const map = {},
      roots = [] as Array<any>;

    for (let i = 0; i < array.length; i += 1) {
      map[array[i].key] = i;
      array[i].children = [];
    }

    for (let i = 0; i < array.length; i += 1) {
      const node = array[i];
      if (node.parentId) {
        array[map[node.parentId]].children.push(node);
      } else {
        roots.push(node);
      }
    }
    return roots;
  }
}
export default new TreeDataSourceUtils();
