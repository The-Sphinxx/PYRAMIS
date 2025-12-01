import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/useAuthStore'

// Layouts
import PublicLayout from '@/Layouts/PublicLayout.vue'
import AdminLayout from '@/Layouts/AdminLayout.vue'

// Public Views
import HomeView from '../Views/Public/HomeView.vue'
import AboutView from '../Views/Public/AboutView.vue'
import BooksListView from '../Views/Public/BooksListView.vue'
import BooksDetailsView from '../Views/Public/BooksDetailsView.vue'
import AuthorsListView from '../Views/Public/AuthorsListView.vue'
import AuthorDetailsView from '../Views/Public/AuthorDetailsView.vue'
import LoginView from '../Views/Public/LoginView.vue'


// Admin Views
import DashboardView from '@/Views/Admin/DashboardView.vue'
import BooksTableView from '@/Views/Admin/BooksTable.vue'
import BookFormView from '@/Views/Admin/BookFormView.vue'
import AuthorsFormView from '@/Views/Admin/AuthorsFormView.vue'
import AuthorsTableView from '@/Views/Admin/AuthorsTableView.vue'
const routes = [
  // Public Routes
  {
    path: '/',
    component: PublicLayout,
    children: [
      {
        path: '',
        name: 'home',
        component: HomeView,
        meta: { title: 'Home' }
      },
      {
        path: 'about',
        name: 'about',
        component: AboutView,
        meta: { title: 'About Us' }
      },
      {
        path: 'books',
        name: 'books',
        component: BooksListView,
        meta: { title: 'Books' }
      },
      {
        path: 'books/:id',
        name: 'book-details',
        component: BooksDetailsView,
        meta: { title: 'Book Details' }
      },
      {
        path: 'authors',
        name: 'authors',
        component: AuthorsListView,
        meta: { title: 'Authors' }
      },
      {
        path: 'authors/:id',
        name: 'author-details',
        component: AuthorDetailsView,
        meta: { title: 'Author Details' }
      }
    ]
  },

  // Login Route 
  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: { title: 'Login', requiresGuest: true }
  },

  // Admin Routes 
  {
    path: '/admin',
    component: AdminLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'admin',
        component: DashboardView,
        meta: { title: 'Admin Dashboard' }
      },
      {
        path: 'books',
        name: 'admin-books',
        component: BooksTableView,
        meta: { title: 'Manage Books' }
      },
      {
        path: 'books/new',
        name: 'admin-books-create',
        component: BookFormView,
        meta: { title: 'Create Book' }
      },
      {
        path: 'books/:id/edit',
        name: 'admin-books-edit',
        component: BookFormView,
        meta: { title: 'Edit Book' }
      },
      {
        path: 'authors',
        name: 'admin-authors',
        component: AuthorsTableView,
        meta: { title: 'Manage Authors' }
      },
      {
        path: 'authors/new',
        name: 'admin-authors-create',
        component: AuthorsFormView,
        meta: { title: 'Create Author' }
      },
      {
        path: 'authors/:id/edit',
        name: 'admin-authors-edit',
        component: AuthorsFormView,
        meta: { title: 'Edit Author' }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) return savedPosition
    return { top: 0, left: 0, behavior: 'smooth' }
  }
})

// Navigation Guards
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  // Check if route requires authentication
  if (to.meta.requiresAuth) {
    if (!authStore.isAuthenticated) {
      // Redirect to login with return URL
      next({
        name: 'login',
        query: { redirect: to.fullPath }
      })
    } else {
      next()
    }
  }
  // Check if route requires guest 
  else if (to.meta.requiresGuest) {
    if (authStore.isAuthenticated) {
      // Redirect to admin dashboard if already logged in
      next({ name: 'admin' })
    } else {
      next()
    }
  }
  // No auth requirements
  else {
    next()
  }
})

export default router
