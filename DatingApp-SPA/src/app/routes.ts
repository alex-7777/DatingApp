import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberListResolver } from './_resolves/member-list.resolver';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolves/member-detail.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolves/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { ListsResolver } from './_resolves/lists.resolver';
import { MessagesResolver } from './_resolves/messages.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';

export const appRoutes: Routes = [
    // { path: 'home', component: HomeComponent },
    { path: '', component: HomeComponent },
    {
        path: '', // path '' + 'members' ....
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent ,
                resolve: {users: MemberListResolver} },
            { path: 'members/:id', component: MemberDetailComponent,
                resolve: {user: MemberDetailResolver} },
            { path: 'member/edit', component: MemberEditComponent,
                resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges] },
            { path: 'messages', component: MessagesComponent,
                resolve: {messages: MessagesResolver} },
            { path: 'lists', component: ListsComponent,
                resolve: {users: ListsResolver} },
            { path: 'admin', component: AdminPanelComponent, data: {roles: ['Admin', 'Moderator']} }
        ]
    },

    // Alternative way of using the Guards for every route separately
    // { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] }

    { path: '**', redirectTo: '', pathMatch: 'full'}
    // { path: '**', redirectTo: 'home', pathMatch: 'full'}
];
