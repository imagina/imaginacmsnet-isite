﻿namespace Isite.Config
{
    public static class CmsSidebar
    {

        private static string cmsSidebar = @"{
            'admin':[
            'isite_cms_main_home',
            {
            'title':'isite.cms.sidebar.adminGroup',
            'icon':'fas fa-chess-rook',
            'children':['isite_cms_admin_organizations','isite_cms_admin_categories','isite_cms_admin_icruds']
            },
            'isite_cms_admin_index'
            ],
            'panel':['isite_cms_main_home']
        }";

        public static string GetCmsSidebar()
        {
            return cmsSidebar;
        }
    }
}
