﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TgBotFrame.Commands.Authorization.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TgBotFrame.Commands.Authorization.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на В выполнении команды отказано, вы не имеете необходимых ролей.
        /// </summary>
        public static string AuthorizationMiddleware_Denied {
            get {
                return ResourceManager.GetString("AuthorizationMiddleware_Denied", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь @{0} заблокирован до {1:F}.
        /// </summary>
        public static string BanController_Ban_Result {
            get {
                return ResourceManager.GetString("BanController_Ban_Result", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Без срока.
        /// </summary>
        public static string BanController_BanInfo_Infinite {
            get {
                return ResourceManager.GetString("BanController_BanInfo_Infinite", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на У данного пользователя нет блокировок.
        /// </summary>
        public static string BanController_BanInfo_NotFound {
            get {
                return ResourceManager.GetString("BanController_BanInfo_NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на До .
        /// </summary>
        public static string BanController_BanInfo_Until {
            get {
                return ResourceManager.GetString("BanController_BanInfo_Until", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь {0} разблокирован.
        /// </summary>
        public static string BanController_UnBan_Result {
            get {
                return ResourceManager.GetString("BanController_UnBan_Result", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Отвечаем списком заблокированных пользователей. Указав параметром идентификатор пользователя, бот отвечает подробностями блокировки.
        /// </summary>
        public static string Category_Description_BanInfo {
            get {
                return ResourceManager.GetString("Category_Description_BanInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Данные команды позволяют управлять блокировками пользователей.
        /// </summary>
        public static string Category_Description_Bans {
            get {
                return ResourceManager.GetString("Category_Description_Bans", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Данные команды позволяют управлять ролями, а также производить некоторые действия с их участниками, например, упоминать их.
        /// </summary>
        public static string Category_Description_Roles {
            get {
                return ResourceManager.GetString("Category_Description_Roles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Блокировки.
        /// </summary>
        public static string Category_Name_Bans {
            get {
                return ResourceManager.GetString("Category_Name_Bans", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Роли.
        /// </summary>
        public static string Category_Name_Roles {
            get {
                return ResourceManager.GetString("Category_Name_Roles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Приписывает к указанный роли пользователя.
        ///Сначала укажите название роли, а затем идентификатор пользователя.
        /// </summary>
        public static string Command_Description_AddRole {
            get {
                return ResourceManager.GetString("Command_Description_AddRole", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Блокирует пользователя внутри бота, не позволяя ему исполнять команды. Различные варианты команды предполагают, что числом является идентификатор пользователя, интервалом — длительность блокировки, а текст — описание причины блокировки для администраторов.
        /// </summary>
        public static string Command_Description_Ban {
            get {
                return ResourceManager.GetString("Command_Description_Ban", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Укажите название роли первым аргументом, чтобы упомянуть всех участников роли. Вызвав команду без аргументов, бот ответит списком ролей, участников которых можно упомянуть. Вызывав команду с двумя аргументами, администраторы бота могут управлять возможностью упоминания роли.
        ///Первым аргументом является идентификатор роли, а вторым - признак возможности упоминания..
        /// </summary>
        public static string Command_Description_Mention {
            get {
                return ResourceManager.GetString("Command_Description_Mention", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Исключает пользователя из участников роли. 
        ///Сначала укажите название роли, а затем идентификатор пользователя..
        /// </summary>
        public static string Command_Description_RemoveRole {
            get {
                return ResourceManager.GetString("Command_Description_RemoveRole", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Досрочно снимает блокировку у пользователя, чей идентификатор был указан.
        /// </summary>
        public static string Command_Description_UnBan {
            get {
                return ResourceManager.GetString("Command_Description_UnBan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на , ваша роль упомянута.
        /// </summary>
        public static string MentionController_Mention {
            get {
                return ResourceManager.GetString("MentionController_Mention", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Возможность упоминания роли {0} теперь .
        /// </summary>
        public static string MentionController_Mention_EditResult {
            get {
                return ResourceManager.GetString("MentionController_Mention_EditResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Возможность упоминания участников ролей:.
        /// </summary>
        public static string MentionController_Mention_ListTitle {
            get {
                return ResourceManager.GetString("MentionController_Mention_ListTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Роль не существует или в ней нет ни одного участника.
        /// </summary>
        public static string MentionController_Mention_NotFound {
            get {
                return ResourceManager.GetString("MentionController_Mention_NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на  **выключена** .
        /// </summary>
        public static string MentionController_Mention_Off {
            get {
                return ResourceManager.GetString("MentionController_Mention_Off", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на  *ВЫКЛ* .
        /// </summary>
        public static string MentionController_Mention_OffShort {
            get {
                return ResourceManager.GetString("MentionController_Mention_OffShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на  **включена** .
        /// </summary>
        public static string MentionController_Mention_On {
            get {
                return ResourceManager.GetString("MentionController_Mention_On", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на  **ВКЛ** .
        /// </summary>
        public static string MentionController_Mention_OnShort {
            get {
                return ResourceManager.GetString("MentionController_Mention_OnShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Роль &quot;{0}&quot; не существует.
        /// </summary>
        public static string RoleManagementController_Add_NotFound {
            get {
                return ResourceManager.GetString("RoleManagementController_Add_NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователю {0} назначена роль &quot;{1}&quot;.
        /// </summary>
        public static string RoleManagementController_Add_Success {
            get {
                return ResourceManager.GetString("RoleManagementController_Add_Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователю {0} не назначена роль &quot;{1}&quot; или таковой не существует.
        /// </summary>
        public static string RoleManagementController_Remove_NotFound {
            get {
                return ResourceManager.GetString("RoleManagementController_Remove_NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь {0} более не имеет роли &quot;{1}&quot;.
        /// </summary>
        public static string RoleManagementController_Remove_Success {
            get {
                return ResourceManager.GetString("RoleManagementController_Remove_Success", resourceCulture);
            }
        }
    }
}
