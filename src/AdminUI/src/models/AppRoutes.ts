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
  Cms_Tickets = "cms-tickets",
  Cms_Attendees = "cms-attendees",
  Cms_New_Attendees = "cms-attendees-new",
  Cms_Cancelled_Attendees = "cms-attendees-cancelled",
  Cms_MessageProviders = "cms-message-providers"
}

export default AppRoutes;
