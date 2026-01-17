import { createI18n } from 'vue-i18n'

const messages = {
  en: {
    sidebar: {
      logo: 'Ticketing System',
      home: 'Home',
      tickets: 'Tickets',
      products: 'Products',
      users: 'Users',
      profile: 'Profile',
      logout: 'Logout'
    },
    home: {
      loading: 'Loading...',
      greeting: 'Hello, {name}!',
      customerSubtitle: "Need help with a product? We're here to help you.",
      staffSubtitle: 'Review and manage customer support tickets.',
      createTicket: 'Create New Ticket',
      browseTickets: 'Browse All Tickets',
      myTickets: 'My Tickets',
      inProgress: 'In Progress',
      resolved: 'Resolved',
      unassigned: 'Unassigned Tickets',
      totalTickets: 'Total Tickets',
      youSolved: 'You have Solved',
      latestActivity: 'Latest Activity',
      forTicket: 'for ticket:',
      viewDetails: 'View Details'
    },
    changePassword: {
      title: 'Account Security',
      subtitle: 'Update Password',
      current: 'Current Password',
      new: 'New Password',
      confirm: 'Confirm New Password',
      button: 'Change Password',
      loading: 'Updating...',
      success: 'Success! Please log in with your new password.',
      placeholders: {
        current: 'Enter current password',
        new: 'Enter new password',
        confirm: 'Repeat new password'
      },
      errors: {
        match: 'Passwords do not match.',
        short: 'Password is too short.',
        failed: 'Update failed. Check your current password.'
      },
      modal: {
        title: 'Confirm Change',
        body: 'You will be logged out to verify the new credentials.',
        confirm: 'Update'
      }
    },
    profile: {
      title: 'Profile Settings',
      accountInfo: 'Account Information',
      changePassword: 'Change Password',
      editProfile: 'Edit Profile',
      cancel: 'Cancel',
      saveChanges: 'Save Changes',
      username: 'Username',
      firstName: 'First Name',
      lastName: 'Last Name',
      email: 'E-mail',
      dialogs: {
        saveTitle: 'Save Changes?',
        saveBody: 'Are you sure you want to update your profile information?',
        saveConfirm: 'Save',
        successTitle: 'Profile Updated',
        successBody: 'Your information has been successfully updated.',
        errorTitle: 'Update Failed',
        inputError: 'Please check your input format.',
        genericError: 'Issue updating profile.'
      }
    },
    "login": {
      "systemStatus": "System Active 24/7",
      "heroTitle": "Resolve issues",
      "heroSubtitle": "faster than ever.",
      "heroLead": "The next-generation ticket management system.",
      "welcome": "Welcome Back",
      "enterDetails": "Enter your details to access your account.",
      "username": "Username",
      "password": "Password",
      "signIn": "Sign In",
      "newHere": "New here?",
      "createAccount": "Create an account",
      "error": "Invalid login credentials."
    },
    "register": {
      "heroTitle": "Begin your",
      "heroSubtitle": "journey with us.",
      "heroLead": "Get the support you deserve, quickly and easily.",
      "title": "Create Account",
      "subtitle": "Sign up to start managing your tickets.",
      "firstName": "First Name",
      "lastName": "Last Name",
      "username": "Username",
      "email": "Email",
      "password": "Password",
      "confirm": "Confirm",
      "submit": "Create Account",
      "alreadyHaveAccount": "Already have an account?",
      "loginHere": "Login here",
      "placeholders": {
        "firstName": "first name",
        "lastName": "last name",
        "username": "choose a username",
        "email": "enter your email"
      },
      "errors": {
        "mismatch": "Passwords do not match",
        "generic": "An unexpected error occurred."
      }
    },
    users: {
      title: 'User Management',
      searchPlaceholder: 'Search users...',
      addStaff: 'Add Staff',
      table: {
        fullName: 'Full Name',
        username: 'Username',
        email: 'Email',
        role: 'Role',
        status: 'Status'
      },
      roles: {
        admin: 'Admin',
        support: 'Support',
        customer: 'Customer'
      },
      status: {
        active: 'Active',
        pending: 'Pending',
        inactive: 'Inactive'
      },
      empty: 'No users found',
      noResults: 'No results for "{query}"',
      pagination: 'Showing {count} of {total}',
      addStaffModal: {
        title: 'Add New Staff Member',
        firstName: 'First Name',
        lastName: 'Last Name',
        email: 'Email',
        username: 'Username',
        successTitle: 'Staff Member Created',
        tempPassword: 'Temporary Password (Please share with the user)',
        done: 'Done',
        create: 'Create',
        placeholders: {
          fn: 'Enter first name',
          ln: 'Enter last name',
          email: 'Enter email address',
          user: 'Assign a username'
        }
      },
      deleteModal: {
        title: 'Delete user?',
        body: 'Delete <strong>{username}</strong>? This action cannot be undone.',
        confirm: 'Delete'
      },
      success: {
        deleted: 'User deleted successfully.'
      },
    },
    tickets: {
      title: 'Tickets',
      searchPlaceholder: 'Search tickets...',
      viewDetails: 'View Details',
      addTicket: 'Add Ticket',
      table: {
        title: 'Title',
        product: 'Product Name',
        priority: 'Priority',
        status: 'Status',
        createdBy: 'Created By',
        assignedTo: 'Assigned To',
        date: 'Create Date'
      },
      status: {
        unassigned: 'Unassigned',
        assignMe: 'Assign Me',
        new: 'New',
        progress: 'In Progress',
        resolved: 'Resolved',
        closed: 'Closed',
        reopened: 'Reopened'
      },
      priority: {
        low: 'Low',
        medium: 'Medium',
        high: 'High'
      },
      empty: 'No ticket found',
      noResults: 'No results for "{query}"',
      pagination: 'Showing {count} of {total}',
      modal: {
        title: 'Create New Ticket',
        ticketTitle: 'Ticket Title',
        description: 'Description',
        product: 'Product',
        attachments: 'Attachments',
        selectProduct: '-- Select Product --',
        placeholders: {
          title: 'Enter title',
          desc: 'Describe the issue'
        }
      }
    },
    products: {
      title: 'Products',
      searchPlaceholder: 'Search products...',
      addProduct: 'Add Product',
      noImage: 'No image',
      empty: 'No products found',
      emptySub: 'Try adjusting your search or add a new product',
      pagination: 'Showing {count} of {total}',
      modal: {
        addTitle: 'Add New Product',
        nameLabel: 'Product Name',
        namePlaceholder: 'Enter product name',
        addButton: 'Add',
        errorRequired: 'Product name is required!',
        successAdd: 'Product: {name} Added successfully!',
        errorAdd: 'Could not add product'
      },
      delete: {
        title: 'Delete Product',
        confirm: 'Are you sure you want to delete <b>{name}</b>?',
        button: 'Delete',
        success: '{name} has been deleted successfully!',
        failed: 'Deletion Failed'
      }
    },
    ticketDetails: {
      backToList: 'Back to List',
      loading: 'Loading ticket data...',
      ticketNumber: 'Ticket Number',
      description: 'Description',
      noDescription: 'No description provided.',
      attachments: 'Attachments',
      sidebarTitle: 'Ticket Details',
      status: 'Status',
      priority: 'Priority',
      product: 'Product',
      agent: 'Agent',
      unassigned: 'Unassigned',
      createdBy: 'Created By',
      date: 'Date',
      tabs: {
        discussion: 'Discussion',
        history: 'Activity History'
      },
      comments: {
        placeholder: 'Write a comment...',
        attach: 'Attach File',
        post: 'Post',
        posting: 'Posting...',
        empty: 'No comments yet. Be the first to start the discussion.',
        success: 'Comment posted!'
      },
      history: {
        system: 'System',
        changedStatus: 'Changed status to'
      }
    }
  },
  ar: {
    sidebar: {
      logo: 'نظام التذاكر',
      home: 'الرئيسية',
      tickets: 'التذاكر',
      products: 'المنتجات',
      users: 'المستخدمين',
      profile: 'الملف الشخصي',
      logout: 'تسجيل الخروج'
    },
    changePassword: {
      title: 'أمان الحساب',
      subtitle: 'تحديث كلمة المرور',
      current: 'كلمة المرور الحالية',
      new: 'كلمة المرور الجديدة',
      confirm: 'تأكيد كلمة المرور الجديدة',
      button: 'تغيير كلمة المرور',
      loading: 'جاري التحديث...',
      success: 'تم بنجاح! يرجى تسجيل الدخول بكلمة المرور الجديدة.',
      placeholders: {
        current: 'أدخل كلمة المرور الحالية',
        new: 'أدخل كلمة المرور الجديدة',
        confirm: 'كرر كلمة المرور الجديدة'
      },
      errors: {
        match: 'كلمات المرور غير متطابقة.',
        short: 'كلمة المرور قصيرة جداً.',
        failed: 'فشل التحديث. تحقق من كلمة المرور الحالية.'
      },
      modal: {
        title: 'تأكيد التغيير',
        body: ' سيتم تسجيل خروجك للتحقق من البيانات الجديدة.',
        confirm: 'تحديث'
      }
    },
    profile: {
      title: 'إعدادات الملف الشخصي',
      accountInfo: 'معلومات الحساب',
      changePassword: 'تغيير كلمة المرور',
      editProfile: 'تعديل الملف',
      cancel: 'إلغاء',
      saveChanges: 'حفظ التغييرات',
      username: 'اسم المستخدم',
      firstName: 'الاسم الأول',
      lastName: 'اسم العائلة',
      email: 'البريد الإلكتروني',
      dialogs: {
        saveTitle: 'حفظ التغييرات؟',
        saveBody: 'هل أنت متأكد أنك تريد تحديث معلومات ملفك الشخصي؟',
        saveConfirm: 'حفظ',
        successTitle: 'تم التحديث',
        successBody: 'تم تحديث معلوماتك بنجاح.',
        errorTitle: 'فشل التحديث',
        inputError: 'يرجى التحقق من صيغة المدخلات.',
        genericError: 'حدثت مشكلة أثناء التحديث.'
      }
    },
    "login": {
      "systemStatus": "النظام متاح 24/7",
      "heroTitle": "حل المشكلات",
      "heroSubtitle": "أسرع من أي وقت مضى.",
      "heroLead": "الجيل القادم من أنظمة إدارة التذاكر.",
      "welcome": "مرحباً بك",
      "enterDetails": "أدخل بياناتك للوصول إلى حسابك.",
      "username": "اسم المستخدم",
      "password": "كلمة المرور",
      "signIn": "تسجيل الدخول",
      "newHere": "جديد هنا؟",
      "createAccount": "أنشئ حساباً",
      "error": "بيانات الدخول غير صالحة."
    },
    "register": {
      "heroTitle": "ابدأ",
      "heroSubtitle": "رحلتك معنا.",
      "heroLead": "احصل على الدعم الذي تستحقه بسرعة وسهولة.",
      "title": "إنشاء حساب",
      "subtitle": "سجل الآن لتبدأ في إدارة تذاكرك.",
      "firstName": "الاسم الأول",
      "lastName": "اسم العائلة",
      "username": "اسم المستخدم",
      "email": "البريد الإلكتروني",
      "password": "كلمة المرور",
      "confirm": "تأكيد كلمة المرور",
      "submit": "إنشاء الحساب",
      "alreadyHaveAccount": "لديك حساب بالفعل؟",
      "loginHere": "سجل دخولك هنا",
      "placeholders": {
        "firstName": "الاسم الأول",
        "lastName": "اسم العائلة",
        "username": "اختر اسم المستخدم",
        "email": "أدخل بريدك الإلكتروني"
      },
      "errors": {
        "mismatch": "كلمات المرور غير متطابقة",
        "generic": "حدث خطأ غير متوقع."
      }
    },
    users: {
      title: 'إدارة المستخدمين',
      searchPlaceholder: 'البحث عن مستخدمين...',
      addStaff: 'إضافة موظف',
      table: {
        fullName: 'الاسم الكامل',
        username: 'اسم المستخدم',
        email: 'البريد الإلكتروني',
        role: 'الدور',
        status: 'الحالة'
      },
      roles: {
        admin: 'مدير',
        support: 'دعم فني',
        customer: 'عميل'
      },
      status: {
        active: 'نشط',
        pending: 'قيد الانتظار',
        inactive: 'غير نشط'
      },
      empty: 'لم يتم العثور على مستخدمين',
      noResults: 'لا توجد نتائج لـ "{query}"',
      pagination: 'عرض {count} من أصل {total}',
      addStaffModal: {
        title: 'إضافة موظف جديد',
        firstName: 'الاسم الأول',
        lastName: 'اسم العائلة',
        email: 'البريد الإلكتروني',
        username: 'اسم المستخدم',
        successTitle: 'تم إنشاء حساب الموظف',
        tempPassword: 'كلمة المرور المؤقتة (يرجى مشاركتها مع المستخدم)',
        done: 'تم',
        create: 'إنشاء',
        placeholders: {
          fn: 'أدخل الاسم الأول',
          ln: 'أدخل اسم العائلة',
          email: 'أدخل البريد الإلكتروني',
          user: 'تعيين اسم مستخدم'
        }
      },
      deleteModal: {
        title: 'حذف المستخدم؟',
        body: 'هل تريد حذف <strong>{username}</strong>؟ لا يمكن التراجع عن هذا الإجراء.',
        confirm: 'حذف'
      },
      success: {
        deleted: 'تم حذف المستخدم بنجاح.'
      }
    },
    home: {
      loading: 'جاري التحميل...',
      greeting: 'مرحباً، {name}!',
      customerSubtitle: 'هل تحتاج إلى مساعدة في منتج ما؟ نحن هنا لمساعدتك.',
      staffSubtitle: 'مراجعة وإدارة تذاكر العملاء.',
      createTicket: 'إنشاء تذكرة جديدة',
      browseTickets: 'تصفح جميع التذاكر',
      myTickets: 'تذاكري',
      inProgress: 'قيد التنفيذ',
      resolved: 'تم الحل',
      unassigned: 'تذاكر غير معينة',
      totalTickets: 'إجمالي تذاكرك',
      youSolved: 'تذاكر قمت بحلها',
      latestActivity: 'آخر الأنشطة',
      forTicket: 'للتذكرة:',
      viewDetails: 'عرض التفاصيل'
    },
    tickets: {
      title: 'التذاكر',
      searchPlaceholder: 'البحث عن التذاكر...',
      viewDetails: 'عرض التفاصيل',
      addTicket: 'إضافة تذكرة',
      table: {
        title: 'العنوان',
        product: 'اسم المنتج',
        priority: 'الأولوية',
        status: 'الحالة',
        createdBy: 'أنشئت بواسطة',
        assignedTo: 'مكلف إلى',
        date: 'تاريخ الإنشاء'
      },
      status: {
        unassigned: 'غير مكلف',
        assignMe: 'تكليفي بها',
        new: 'جديدة',
        progress: 'قيد التنفيذ',
        resolved: 'تم الحل',
        closed: 'مغلقة',
        reopened: 'أعيد فتحها'
      },
      priority: {
        low: 'منخفضة',
        medium: 'متوسطة',
        high: 'عالية'
      },
      empty: 'لم يتم العثور على تذاكر',
      noResults: 'لا توجد نتائج لـ "{query}"',
      pagination: 'عرض {count} من أصل {total}',
      modal: {
        title: 'إنشاء تذكرة جديدة',
        ticketTitle: 'عنوان التذكرة',
        description: 'الوصف',
        product: 'المنتج',
        attachments: 'المرفقات',
        selectProduct: '-- اختر المنتج --',
        placeholders: {
          title: 'أدخل العنوان',
          desc: 'صف المشكلة بالتفصيل'
        }
      }
    },
    products: {
      title: 'المنتجات',
      searchPlaceholder: 'البحث عن المنتجات...',
      addProduct: 'إضافة منتج',
      noImage: 'لا توجد صورة',
      empty: 'لم يتم العثور على منتجات',
      emptySub: 'حاول تعديل البحث أو إضافة منتج جديد',
      pagination: 'عرض {count} من أصل {total}',
      modal: {
        addTitle: 'إضافة منتج جديد',
        nameLabel: 'اسم المنتج',
        namePlaceholder: 'أدخل اسم المنتج',
        addButton: 'إضافة',
        errorRequired: 'اسم المنتج مطلوب!',
        successAdd: 'تم إضافة المنتج: {name} بنجاح!',
        errorAdd: 'تعذر إضافة المنتج'
      },
      delete: {
        title: 'حذف المنتج',
        confirm: 'هل أنت متأكد أنك تريد حذف <b>{name}</b>؟',
        button: 'حذف',
        success: 'تم حذف {name} بنجاح!',
        failed: 'فشل الحذف'
      }
    },
    ticketDetails: {
      backToList: 'العودة للقائمة',
      loading: 'جاري تحميل بيانات التذكرة...',
      ticketNumber: 'رقم التذكرة',
      description: 'الوصف',
      noDescription: 'لا يوجد وصف متاح.',
      attachments: 'المرفقات',
      sidebarTitle: 'تفاصيل التذكرة',
      status: 'الحالة',
      priority: 'الأولوية',
      product: 'المنتج',
      agent: 'مكلفة الى',
      unassigned: 'غير مكلف',
      createdBy: 'أنشئت بواسطة',
      date: 'التاريخ',
      tabs: {
        discussion: 'المناقشة',
        history: 'سجل النشاطات'
      },
      comments: {
        placeholder: 'اكتب تعليقاً...',
        attach: 'إرفاق ملف',
        post: 'نشر',
        posting: 'جاري النشر...',
        empty: 'لا توجد تعليقات بعد. كن أول من يبدأ المناقشة.',
        success: 'تم نشر التعليق!'
      },
      history: {
        system: 'النظام',
        changedStatus: 'قام بتغيير الحالة إلى'
      }
    }
  }
}

export const i18n = createI18n({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
  globalInjection: true,
  messages,
})
