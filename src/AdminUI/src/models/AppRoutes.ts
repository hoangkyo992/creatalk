enum AppRoutes {
  Home = "index",
  Login = "login",
  Forbidden = "403",
  Dashboard = "dashboard",

  System = "system",
  System_Users = "system-users",
  System_Roles = "system-roles",
  System_UserSessions = "system-user-sessions",
  System_Activities = "system-activities",

  System_Settings_Media = "settings-media",
  System_Settings = "settings-general",
  System_Settings_SocialNetwork = "settings-social-networks",

  Cdn = "cdn",
  Cdn_Library = "cdn-library",
  Cdn_Albums = "cdn-albums",

  Cms = "cms",
  Cms_Pages = "cms-pages",
  Cms_PageId = "cms-pages-[id]",
  Cms_Posts = "cms-posts",
  Cms_PostId = "cms-posts-[id]",
  Cms_News = "cms-posts-news",
  Cms_NewsId = "cms-posts-news-[id]",
  Cms_Products = "cms-posts-products",
  Cms_ProductId = "cms-posts-products-[id]",
  Cms_Projects = "cms-posts-projects",
  Cms_ProjectId = "cms-posts-projects-[id]",
  Cms_Factories = "cms-posts-factories",
  Cms_FactoryId = "cms-posts-factories-[id]",
  Cms_Sections = "cms-posts-sections",
  Cms_SectionId = "cms-posts-sections-[id]",
  Cms_Jobs = "cms-posts-jobs",
  Cms_JobId = "cms-posts-jobs-[id]",
  Cms_Events = "cms-posts-events",
  Cms_EventId = "cms-posts-events-[id]",
  Cms_Templates = "cms-templates",
  Cms_Languages = "cms-languages",
  Cms_Attributes = "cms-attributes",
  Cms_UIComponents = "cms-ui-components",
  Cms_Tags = "cms-tags",
  Cms_Menus = "cms-menus",
  Cms_Categories = "cms-categories",

  Rms = "rms",
  Rms_Candidates = "rms-candidates",

  Crm = "crm",
  Crm_Contacts = "crm-contacts"
}

export default AppRoutes;
