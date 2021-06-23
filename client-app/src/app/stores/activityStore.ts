import { format } from "date-fns";
import { makeAutoObservable, reaction, runInAction} from "mobx"
import agent from "../api/agent";
import { Activity, ActivityFormValues } from "../models/activity";
import { Pagination, PagingParams } from "../models/pagination";
import { Profile } from "../models/profile";
import { store } from "./store";

export default class ActivityStore {
    /*Dictionary of activities that are kept in memory for better performance*/
    activityRegistry = new Map<string, Activity>();
    /*Activity selected when clicked on view button */
    selectedActivity: Activity | undefined = undefined;
    /*Displays form*/
    editMode = false;
    /*Loading indicator on buttons*/
    loading = false;
    /*Loading screen*/
    loadingInitial = false;
    pagination: Pagination | null = null;
    pagingParams = new PagingParams();
    //Containing all posible values from different filters
    predicate = new Map().set('all', true);

    constructor () {
        /*Bind properties to class*/
        makeAutoObservable(this)

        //React to every time predicate keys change
        reaction(
            () => this.predicate.keys(), 
            () => {
                //Restart values
                this.pagingParams = new PagingParams();
                this.activityRegistry.clear();
                this.loadActivities();
            }
        )
    }

    setPagingParams = (PagingParams: PagingParams) => {
        this.pagingParams = PagingParams;
    }

    setPredicate = (predicate: string, value: string | Date) => {
        //Switch off all other predicates apart from the date
        const resetPredicate = () => {
            this.predicate.forEach((value, key) => {
                if (key !== 'startDate') this.predicate.delete(key);
            })
        }
        //Verify what kind of filter user wants
        switch (predicate) {
            case 'all':
                resetPredicate();
                this.predicate.set('all', true);
                break;
            case 'isGoing':
                resetPredicate();
                this.predicate.set('isGoing', true);
                break;
            case 'isHost':
                resetPredicate();
                this.predicate.set('isHost', true);
                break;
            case 'startDate':
                this.predicate.delete('startDate');
                this.predicate.set('startDate', value);
                break;
        }
    }

    get axiosparams() {
        const params = new URLSearchParams();
        //Add query string parameters to the object and then add object to axios
        params.append('pageNumber', this.pagingParams.pageNumber.toString());
        params.append('pageSize', this.pagingParams.pageSize.toString());
        this.predicate.forEach((value, key) => {
            if (key === 'startDate') {
                params.append(key, (value as Date).toISOString())
            } else {
                params.append(key, value);
            }
        })
        return params;
    }

    get activitiesByDate() {
        return Array.from(this.activityRegistry.values()).sort((a, b) =>
            a.date!.getTime() - b.date!.getTime());
    }

    get groupActivities () {
        /*Returning an array of activities that match a date */
        /*Use each date inside every activity as a key */
        return Object.entries(
            this.activitiesByDate.reduce((activities, activity) => {
                const date = format(activity.date!, 'dd MMM yyyy');
                activities[date] = activities[date] ? [...activities[date], activity] : [activity]
                return activities;
            }, {} as {[key: string]: Activity[]})
        )
    }

    /*Arrow functions automatically bind the method to the class */
    loadActivities = async () => {
        this.loadingInitial = true;
        try {
            const result = await agent.Activities.list(this.axiosparams);
            result.data.forEach(activity => {
                this.setActivity(activity);
            });
            this.setPagination(result.pagination);
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    }

    setPagination = (pagination: Pagination) => {
        this.pagination = pagination;
    }

    loadActivity = async (id: string) => {
        /*Verify is activity exists in dictionary*/
        let activity = this.getActivity(id);
        if(activity) {
            this.selectedActivity = activity;
            return activity;
        } else {
            this.loadingInitial = true;
            try {
                /*Bring from API*/
                activity = await agent.Activities.details(id);
                this.setActivity(activity);
                runInAction(() => {
                    this.selectedActivity = activity;
                })
                this.setLoadingInitial(false);
                return activity;
            } catch(error){
                this.setLoadingInitial(false);
            }
        }
    }

    private setActivity = (activity: Activity) => {
        //Verify if user is logged in
        const user = store.userStore.user;
        if (user) {
            //Verify if user is going to current activity
            activity.isGoing = activity.attendees!.some(
                a => a.username === user.username
            )
            activity.isHost = activity.hostUsername === user.username;
            activity.host = activity.attendees?.find(x => x.username === activity.hostUsername);
        }
        activity.date = new Date(activity.date!)
        /*Insert into dictionary*/
        this.activityRegistry.set(activity.id, activity);
    }

    private getActivity = (id: string) => {
        /*Verify is activity exists in dictionary*/
        return this.activityRegistry.get(id);
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    createActivity = async (activity: ActivityFormValues) => {
        const user = store.userStore.user;
        const attendee = new Profile(user!);
        try {
            await agent.Activities.create(activity);
            const newActivity = new Activity(activity);
            newActivity.hostUsername = user!.username;
            newActivity.attendees = [attendee];
            this.setActivity(newActivity);
            /*RunInAction is necessary when state properties' values are changed. Otherwise, it is necessary to create methods like setLoadingInitial*/
            runInAction(() => {
                this.selectedActivity = newActivity;
            })
        } catch (error){
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    updateActivity = async (id: string, activity: ActivityFormValues) => {
        try {
            await agent.Activities.update(id, activity);
            runInAction(() => {
                if (activity.id) {
                    let updatedActivity = {...this.getActivity(activity.id), ...activity};
                    this.activityRegistry.set(activity.id, updatedActivity as Activity);
                    this.selectedActivity = updatedActivity as Activity;
                }
            })
        } catch(error) {
            console.log(error);
        }
    }

    deleteActivity = async (id: string) => {
        this.loading = true;
        try {
            await agent.Activities.delete(id);
            runInAction(() => {
                this.activityRegistry.delete(id);
                this.loading = false;
            })
        } catch(error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    updateAttendance = async () => {
        const user = store.userStore.user;
        this.loading = true;
        try {
            await agent.Activities.attend(this.selectedActivity!.id);
            runInAction(() => {
                //Current user is going then remove from attendees
                if (this.selectedActivity?.isGoing) {
                    this.selectedActivity.attendees = 
                        this.selectedActivity.attendees?.filter(a => a.username !== user?.username);
                    this.selectedActivity.isGoing = false;
                } else {
                    const attendee = new Profile(user!);
                    this.selectedActivity?.attendees?.push(attendee);
                    this.selectedActivity!.isGoing = true;
                }
                this.activityRegistry.set(this.selectedActivity!.id, this.selectedActivity!);
            })
        } catch (error) {
            console.log(error)
        } finally {
            runInAction(() => this.loading = false)
        }
    }

    cancelActivityToggle = async () => {
        this.loading = true;
        try {
            await agent.Activities.attend(this.selectedActivity!.id);
            runInAction(() => {
                this.selectedActivity!.isCancelled = !this.selectedActivity?.isCancelled;
                this.activityRegistry.set(this.selectedActivity!.id, this.selectedActivity!);
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    clearSelectedActivity = () => {
        this.selectedActivity = undefined;
    }

    updateAttendeeFollowing = (username: string) => {
        this.activityRegistry.forEach(activity => {
            activity.attendees.forEach(attendee => {
                if (attendee.username === username) {
                    attendee.following ? attendee.followersCount-- : attendee.followersCount++;
                    attendee.following = !attendee.following;
                }
            })
        })
    }
}