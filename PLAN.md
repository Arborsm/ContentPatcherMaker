## 目标

构建一个基于 Avalonia 的跨平台桌面应用，支持可视化创建/编辑 Stardew Valley 的 ContentPatcher 模组内容包（扫描 /md 文档提供知识），具备：
- 错误处理与参数校验
- 图形化工作流界面
- 扩展插件 API（第三方参数类型）
- 完整本地化（中/英切换）
- 实时预览
- 规范 JSON 输出
- 友好直观的 UI/UX
- 兼容性严格校验
- 跨平台（Win/macOS/Linux）

---

## 阶段与任务

1) 基础设施与规划
- 定义解决方案目录结构与模块边界（Core、Services、Plugins、UI）
- 引入 DI（Microsoft.Extensions.DependencyInjection）与日志接口
- 统一错误模型与验证器（System.ComponentModel.DataAnnotations）
- 规划 i18n 资源结构与切换机制

2) 文档扫描与知识层
- 扫描 /md 目录，解析 Markdown 标题/段落/表格
- 建立 KnowledgeIndex（键：主题 -> 文档片段）
- 为 UI 提供可检索的帮助与字段说明

3) 数据模型与规范导出
- 定义 ContentPatcher 基础模型（manifest.json、content.json 结构）
- 支持 Patch、Action、Conditions、Tokens 等
- 新建 JSON 序列化器与 Schema 校验（Newtonsoft.Json + 自建校验规则）

4) UI 工作流
- 主窗口：项目管理区 + 配置编辑区 + 预览区 + 辅助文档区
- 表单驱动参数编辑（含验证提示）
- 变更追踪与脏标记

5) 插件扩展 API
- 定义 IParameterTypeProvider 与 IParameterEditorFactory 接口
- 插件发现与加载（约定目录 /Plugins，后续可支持反射加载）
- 将新类型注册到类型系统并在 UI 自动渲染

6) 本地化（中/英）
- 使用资源字典与绑定切换
- 所有可视字符串抽离到资源文件

7) 实时预览
- 基于当前模型生成临时 JSON 与差异视图
- 预留与实际游戏联动的 Hook（不内置游戏）

8) 兼容性校验
- 静态规则：字段必填/类型/范围
- 语义规则：Patch/Action/When 条件组合合法性
- 版本/目标平台约束

9) 打包与分发
- dotnet publish -r win-x64/osx-x64/linux-x64
- 自定义图标与版本信息

---

## 目录结构（计划）

ContentPatcherMaker/
- Core/
  - Models/ (ContentPatcher 模型)
  - Validation/ (验证器)
  - Serialization/ (JSON 导出)
- Services/
  - Docs/ (Markdown 扫描与索引)
  - Preview/ (实时预览生成)
  - Plugins/ (插件载入、类型注册)
- UI/
  - Views/, ViewModels/
  - Resources/ (i18n)
- Plugins/ (第三方扩展放置)

---

## 近期里程碑

- 里程碑 A：DI + 错误模型 + 基础 UI + i18n 框架
- 里程碑 B：md 扫描服务 + 模型 + 导出
- 里程碑 C：工作流界面 + 实时预览
- 里程碑 D：插件 API + 兼容校验
- 里程碑 E：打包与验收

---

## 验收清单（对应需求）

1. 错误处理和参数验证：统一异常、界面验证、高亮提示
2. 图形化工作流界面：可新增/编辑 Patch/Action，直观层级
3. 扩展 API：通过接口/工厂注册新类型 + 示例插件
4. 本地化：中/英切换按钮，资源字典即时刷新
5. 实时预览：右侧 JSON 视图随改动刷新
6. JSON 输出：符合 ContentPatcher 规范，manifest.json + content.json
7. 友好UI：表单分组、帮助提示、键盘操作
8. 兼容性：静态/语义校验与报告
9. 跨平台：三平台发布构建

