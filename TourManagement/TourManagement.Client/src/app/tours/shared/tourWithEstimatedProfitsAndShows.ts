import { TourWithEstimatedProfits} from './tourWithEstimatedProfits';
import { Show } from '../shows/shared/show.model';

export class TourWithEstimatedProfitsAndShows extends TourWithEstimatedProfits{
    shows:Show[];
}