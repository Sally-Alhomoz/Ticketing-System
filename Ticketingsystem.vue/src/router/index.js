import { createRouter, createWebHistory } from 'vue-router'

import AppLayout from '../Layout/AppLayout.vue'
import Login from '../Views/Account View/Login.vue'
import Register from '../Views/Account View/Register.vue'
import Home from '../Views/Home.vue'
import Users from '../Views/Account View/UserList.vue'
import Products from '../Views/Product View/Products.vue'
import Tickets from '../Views/Ticket View/Tickets.vue'
import TicketDetails from '../Views/Ticket View/TicketDetails.vue'

const routes = [
  {
    path: '/',
    name: 'Login',
    component: Login
  },
  {
    path: '/register',
    name: 'Register',
    component: Register
  },
  {
    path: '/app',
    component: AppLayout,

    children: [
      {
        path: 'home',
        name: 'Home',
        component: Home
      },
      {
        path: 'users',
        name: 'UserList',
        component: Users
      },
      {
        path: 'products',
        name: 'Products',
        component: Products
      },
      {
        path: 'tickets',
        name: 'Tickets',
        component: Tickets
      },
      {
        path: 'ticket/:id',
        name: 'TicketDetails',
        component: TicketDetails,
        props: true
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
