import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolves/member-detail.resolver';

export const appRoutes: Routes = [
    // { path: 'home', component: HomeComponent },
    { path: '', component: HomeComponent },
    {
        path: '', // path '' + 'members' ....
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent },
            { path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailResolver} },
            { path: 'messages', component: MessagesComponent },
            { path: 'lists', component: ListsComponent },
        ]
    },

    // Alternative way of using the Guards for every route separately
    // { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] }

    { path: '**', redirectTo: '', pathMatch: 'full'}
    // { path: '**', redirectTo: 'home', pathMatch: 'full'}
];
